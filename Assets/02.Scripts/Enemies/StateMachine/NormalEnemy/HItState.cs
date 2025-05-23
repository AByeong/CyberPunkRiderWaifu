using DG.Tweening;
using UnityEngine;

public class HItState : BaseNormalEnemyState
{
    private float _hitTimer;

    private Sequence _airSequence;
    private float _maxAirHeight = 5f;    // 최대 띄울 수 있는 높이
    private float _airRiseAmount = 1f; // 한 번 띄울 때 높이
    private float _airRiseTime = 0.2f;   // 공중에 띄우는 시간
    private float _hangTime = 0.15f;     // 공중 체류시간
    private float _fallTime = 0.5f;      // 떨어지는 시간

    private Vector3 _knockbackDir;            // 넉백 방향
    private float _knockbackDistance = 0;     // 넉백 거리
    private float _knockbackTime = 0f;        // 넉백 실행 시간
    private float _knockbackAirbonCoeff = 0f; // 공중 피격시 넉백 강도

    

    public override void OnEnter()
    {
        base.OnEnter();

        Owner.IsHit = false;
        Owner.NavMeshAgent.enabled = false;

        
        
        Vector3 direction = (Owner.transform.position - Owner.Target.transform.position).normalized;
        direction.y = 0;
        _knockbackDir = direction;

        if (Owner.TakedDamageValue >= Owner.EnemyData.InAirThreshold)
        {
            float currentY = Owner.transform.position.y;
            float desiredY = Mathf.Min(currentY + _airRiseAmount, _maxAirHeight);

            if (desiredY > currentY)
            {
                Owner.IsInAir = true;
                Owner.Animator.SetFloat("DownType", Random.Range(0, 4));
                Owner.Animator.SetTrigger("OnDown");
                PlayAirborneKnockbackSequence(desiredY, _knockbackDir);
            }
            else
            {
                PlayKnockbackOnly(_knockbackDir);
            }
        }
        else
        {
            Owner.Animator.SetFloat("HitType", Random.Range(1, 3));
            Owner.Animator.SetTrigger("OnHit");
            
            
            
        }
    }
    
    private void PlayAirborneKnockbackSequence(float toY, Vector3 knockbackDir)
    {
        Vector3 startPos = Owner.transform.position;
        Vector3 knockbackTarget = startPos + knockbackDir * _knockbackDistance * _knockbackAirbonCoeff;
        knockbackTarget.y = toY;

        Vector3 fallPos = new Vector3(knockbackTarget.x, 1.7f, knockbackTarget.z);  // TODO : RAY로 지면 높이 확인하기

        _airSequence = DOTween.Sequence();
        _airSequence.Append(Owner.transform.DOMove(knockbackTarget, _airRiseTime).SetEase(Ease.OutSine)) // 띄우기 + 밀기
                    .AppendInterval(_hangTime)
                    .Append(Owner.transform.DOMoveY(fallPos.y, _fallTime).SetEase(Ease.InQuad))
                    .OnComplete(() =>
                    {
                        Owner.IsInAir = false;
                        SuperMachine.ChangeState<DownedState>();
                    });
    }

    private void PlayKnockbackOnly(Vector3 knockbackDir)
    {
        Vector3 knockbackTarget = Owner.transform.position + knockbackDir * _knockbackDistance;

        Owner.Animator.SetFloat("HitType", Random.Range(1, 3));
        Owner.Animator.SetTrigger("OnHit");

        _airSequence = DOTween.Sequence();
        _airSequence.Append(Owner.transform.DOMove(knockbackTarget, _knockbackTime).SetEase(Ease.OutQuad))
                    .AppendInterval(Owner.EnemyData.StaggerTime - _knockbackTime)
                    .OnComplete(() =>
                    {
                        SuperMachine.ChangeState<IdleState>();
                    });
    }

    public override void Update()
    {
        base.Update();
       
        _hitTimer += Time.deltaTime;
        if (_hitTimer >= Owner.EnemyData.StaggerTime)
        {
            SuperMachine.ChangeState<IdleState>();
            return;
        }
    }
}
