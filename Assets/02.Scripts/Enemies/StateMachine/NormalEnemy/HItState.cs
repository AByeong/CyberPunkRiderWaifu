using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;


public class HItState : BaseNormalEnemyState
{
    // 타이머 및 시퀀스
    private float _hitTimer;
    private Sequence _airSequence;


    // --- 넉백/공중띄우기 관련 변수 ---
    [Header("Knockback & Airborne")]
    [SerializeField] private float _maxAirHeight = 3f;
    [SerializeField] public float _airRiseAmount = 2f;
    [SerializeField] private float _airRiseTime = 0.2f;
    [SerializeField] private float _hangTime = 0.5f;
    [SerializeField] private float _fallTime = 0.8f;

    private Vector3 _knockbackDir;
    [SerializeField] private float _knockbackTime = 0.2f;
    [SerializeField] private float _knockbackAirbonCoeff = 0;

    public Damage GettedDamage;
    // --- 넉백/공중띄우기 관련 변수 끝 ---


    // --- 바닥 감지 Raycast 관련 변수 ---
    [Header("Ground Detection for Landing")]
    [SerializeField] private float groundRaycastDistance = 10f;
    [SerializeField] private float landingYOffset = 1f; // 캐릭터 피봇에 따라 조정
    [SerializeField] private float defaultFallbackLandY = 0.7f; // 바닥 못찾을 시 기본 Y
    // --- 바닥 감지 Raycast 관련 변수 끝 ---


    public override void OnEnter()
    {
        base.OnEnter();

        Owner.IsHit = false;
        _hitTimer = 0f;

        if (Owner.TakedDamage.From.CompareTag("Player"))
        {
            SoundManager.Instance.Play(SoundType.NormalEnemy_Hit);
        }

        if (Owner.Animator != null) Owner.Animator.updateMode = AnimatorUpdateMode.UnscaledTime;


        if (Owner.NavMeshAgent != null && Owner.NavMeshAgent.isOnNavMesh && Owner.NavMeshAgent.enabled)
        {
            Owner.NavMeshAgent.isStopped = true;
            Owner.NavMeshAgent.enabled = false;
        }

        if (Owner.Target != null)
        {
            Vector3 direction = (Owner.transform.position - Owner.Target.transform.position).normalized;
            direction.y = 0;
            _knockbackDir = direction;
        }
        else
        {
            _knockbackDir = -Owner.transform.forward;
        }

        if (Owner.TakedDamage.DamageType == EDamageType.Airborne)
        {
            float currentY = Owner.transform.position.y;
            float desiredY = Mathf.Min(currentY + _airRiseAmount, _maxAirHeight);

            Owner.IsInAir = true;
            if (Owner.Animator != null)
            {
                Owner.Animator.SetFloat("DownType", Random.Range(0, 4));
                Owner.Animator.SetTrigger("OnDown");
            }

            PlayAirborneKnockbackSequence(desiredY, _knockbackDir);
        }
        else
        {
            Owner.IsInAir = false;
            if (Owner.Animator != null)
            {
                Owner.Animator.SetFloat("HitType", Random.Range(1, 3));
                Owner.Animator.SetTrigger("OnHit");
            }
            PlayKnockbackOnly(_knockbackDir);
        }
    }

    public override void Update()
    {
        base.Update();
        _hitTimer += Time.deltaTime; // 일반 deltaTime 사용 (게임 시간에 따른 상태 지속시간)

        float currentStaggerTime = Owner.EnemyData.StaggerTime;

        if (_hitTimer >= currentStaggerTime)
        {
            if ((_airSequence == null || !_airSequence.IsActive()) && !Owner.IsInAir)
            {
                if (SuperMachine != null) SuperMachine.ChangeState<IdleState>();
            }
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        if (Owner.Animator != null) Owner.Animator.updateMode = AnimatorUpdateMode.Normal;

        _airSequence?.Kill();
        _hitTimer = 0f;

        // if (Owner.NavMeshAgent != null)
        // {
        //     if (!Owner.NavMeshAgent.enabled) Owner.NavMeshAgent.enabled = true;
        //     if (Owner.NavMeshAgent.isOnNavMesh) Owner.NavMeshAgent.Warp(Owner.transform.position);
        //     // Owner.NavMeshAgent.isStopped = false;
        // }
    }
    

        private void PlayAirborneKnockbackSequence(float toY, Vector3 knockbackDir)
    {
        Vector3 startPos = Owner.transform.position;
        Vector3 knockbackMove = knockbackDir * Owner.TakedDamage.DamageForce * _knockbackAirbonCoeff;
        Vector3 riseTarget = new Vector3(startPos.x + knockbackMove.x, toY, startPos.z + knockbackMove.z);

        float finalFallY = defaultFallbackLandY;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(startPos, out hit, groundRaycastDistance, NavMesh.AllAreas))
        {
            finalFallY = hit.position.y + landingYOffset;
        }
        Vector3 fallPos = new Vector3(riseTarget.x, finalFallY, riseTarget.z);


        _airSequence?.Kill();
        _airSequence = DOTween.Sequence();
        _airSequence.Append(Owner.transform.DOMove(riseTarget, _airRiseTime).SetEase(Ease.OutSine))
            .AppendInterval(_hangTime)
            .Append(Owner.transform.DOMove(fallPos, _fallTime).SetEase(Ease.InQuad))
            .OnComplete(() =>
            {
                Owner.IsInAir = false;
                Owner.transform.position = fallPos; // 최종 위치 보정
                if (SuperMachine != null) SuperMachine.ChangeState<DownedState>();
            });
    }

    private void PlayKnockbackOnly(Vector3 knockbackDir)
    {
        Vector3 startPos = Owner.transform.position;
        Vector3 knockbackTarget = startPos + knockbackDir * Owner.TakedDamage.DamageForce;

        _airSequence?.Kill();
        _airSequence = DOTween.Sequence();
        _airSequence.Append(Owner.transform.DOMove(knockbackTarget, _knockbackTime).SetEase(Ease.OutQuad));

        float staggerDuration = Owner.EnemyData.StaggerTime;
        float remainingStagger = staggerDuration - _knockbackTime;

        if (remainingStagger > 0)
        {
            _airSequence.AppendInterval(remainingStagger);
        }

        _airSequence.OnComplete(() =>
        {
            if (SuperMachine != null) SuperMachine.ChangeState<IdleState>();
        });
    }

}
