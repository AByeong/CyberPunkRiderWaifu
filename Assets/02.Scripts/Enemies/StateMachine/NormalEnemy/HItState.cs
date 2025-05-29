using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class HItState : BaseNormalEnemyState
{
    // 타이머 및 시퀀스
    private float _hitTimer;
    private Sequence _airSequence;

    // --- 피격 머테리얼 효과 관련 변수 ---
    private Coroutine _hitFlashCoroutine;

    [Header("Hit Flash Material Effect")]
    [Tooltip("효과를 적용할 머티리얼의 이름 (예: Boid). Owner의 MaterialName 사용 시 비워둘 수 있음")]
    public string targetMaterialName = "KyleRobot"; // 기본값 예시

    [Tooltip("머티리얼의 메인 색상 프로퍼티 이름 (예: _BaseColor, _Color)")]
    public string mainColorPropertyName = "_Color"; // 대부분 셰이더에서 _Color 또는 _BaseColor 사용

    [Tooltip("피격 시 밝기(V) 값. 높을수록 희게 보임 (HDR 고려 시 1.0 이상)")]
    [Range(0f, 5f)]
    public float highlightVValue = 2.0f;

    [Tooltip("피격 후 잠시 유지될 밝기(V) 값. 원래 머티리얼의 V값과 유사하게 설정")]
    [Range(0f, 5f)]
    public float normalVValue = 0.8f; // 예시 값, 실제 머티리얼 밝기에 맞춰 조정

    [Tooltip("피격 효과 지속 시간 (초). StaggerTime 보다 짧게 설정될 수 있음")]
    public float hitFlashEffectDuration = 0.3f;

    private List<Renderer> _targetedRenderers;
    private MaterialPropertyBlock _propertyBlock;
    private Dictionary<Renderer, Color> _initialRendererColors = new Dictionary<Renderer, Color>();
    // --- 피격 머테리얼 효과 관련 변수 끝 ---

    // --- 넉백/공중띄우기 관련 변수 ---
    [Header("Knockback & Airborne")]
    [SerializeField] private float _maxAirHeight = 3f;
    [SerializeField] public float _airRiseAmount = 2f;
    [SerializeField] private float _airRiseTime = 0.2f;
    [SerializeField] private float _hangTime = 0.5f;
    [SerializeField] private float _fallTime = 0.8f;

    private Vector3 _knockbackDir;
    // [SerializeField] private float _knockbackDistance = 2.0f;
    [SerializeField] private float _knockbackTime = 0.2f;
    [SerializeField] private float _knockbackAirbonCoeff = 0;

    public Damage GettedDamage;
    // --- 넉백/공중띄우기 관련 변수 끝 ---

    // --- 공중 상태 콜라이더 제어 관련 변수 ---
    [Header("Airborne Collider Settings")]
    [Tooltip("공중에 떠 있는 동안 콜라이더가 활성화되어 있는 시간 (초).")]

    // --- 공중 상태 콜라이더 제어 관련 변수 끝 ---

    // --- 바닥 감지 Raycast 관련 변수 ---
    [Header("Ground Detection for Landing")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float raycastOriginOffsetY = 0.1f;
    [SerializeField] private float groundRaycastDistance = 10f;
    [SerializeField] private float landingYOffset = 1f; // 캐릭터 피봇에 따라 조정
    [SerializeField] private float defaultFallbackLandY = 0.7f; // 바닥 못찾을 시 기본 Y
    // --- 바닥 감지 Raycast 관련 변수 끝 ---

    private void CacheRenderersAndInitialColors()
    {
        if (Owner == null) return;


        if (!string.IsNullOrEmpty(Owner.MaterialName))
        {
            targetMaterialName = Owner.MaterialName;
        }

        if (string.IsNullOrEmpty(targetMaterialName))
        {
            // Debug.LogWarning($"[HitState] CacheRenderersAndInitialColors: targetMaterialName is not set for {Owner.gameObject.name}. Hit flash may not work.");
            return;
        }

        if (_targetedRenderers == null) _targetedRenderers = new List<Renderer>();
        _targetedRenderers.Clear();
        _initialRendererColors.Clear();

        Renderer[] allRenderers = Owner.GetComponentsInChildren<Renderer>(true);
        
        foreach (Renderer rend in allRenderers)
        {
            if (rend != null && rend.sharedMaterials != null)
            {
                foreach (Material mat in rend.sharedMaterials)
                {
                    // 머티리얼 이름에 targetMaterialName이 포함되어 있고, (Instance)가 붙는 경우도 처리
                    if (mat != null && mat.name.Contains(targetMaterialName))
                    {
                        if (mat.HasProperty(mainColorPropertyName))
                        {
                            if (!_targetedRenderers.Contains(rend)) _targetedRenderers.Add(rend);
                            if (!_initialRendererColors.ContainsKey(rend)) // 첫 번째 일치하는 머티리얼의 색상 저장
                            {
                                _initialRendererColors[rend] = mat.GetColor(mainColorPropertyName);
                            }
                        }
                        break; // 해당 렌더러에서 일치하는 머티리얼 하나 찾으면 다음 렌더러로
                    }
                }
            }
        }

        if (_targetedRenderers.Count == 0)
        {
            // Debug.LogWarning($"[HitState] CacheRenderersAndInitialColors: No renderers found on {Owner.gameObject.name} using material '{targetMaterialName}' with property '{mainColorPropertyName}'. Hit flash will not work.");
        }

        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _hitTimer = 0f;

        if (Owner.TakedDamage.From.CompareTag("Player"))
        {
            SoundManager.Instance.Play(SoundType.NormalEnemy_Hit);
        }

        if (Owner.Animator != null) Owner.Animator.updateMode = AnimatorUpdateMode.UnscaledTime;

        #region 피격 머테리얼 효과
        // --- 피격 머테리얼 효과 시작 ---
        CacheRenderersAndInitialColors();
        if (_targetedRenderers != null && _targetedRenderers.Count > 0 && _initialRendererColors.Count > 0)
        {
            if (_hitFlashCoroutine != null) StopCoroutine(_hitFlashCoroutine);

            float staggerDuration = (Owner.EnemyData != null && Owner.EnemyData.StaggerTime > 0) ? Owner.EnemyData.StaggerTime : hitFlashEffectDuration;
            float currentFlashDuration = Mathf.Min(staggerDuration, hitFlashEffectDuration);

            if (currentFlashDuration > 0)
            {
                _hitFlashCoroutine = StartCoroutine(HitFlashEffectCoroutine(currentFlashDuration));
            }
            else // 지속시간이 0이면 즉시 normalVValue 적용 (또는 원래 색상)
            {
                SetRenderersColorToVValue(normalVValue);
            }
        }
        // --- 피격 머테리얼 효과 끝 ---
        #endregion

        Owner.IsHit = false;
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

    private IEnumerator HitFlashEffectCoroutine(float duration)
    {
        // 1. 즉시 highlightVValue로 변경
        SetRenderersColorToVValue(highlightVValue);

        // 2. 짧은 시간 동안 최대 밝기 유지 (예: 전체 duration의 20%)
        float peakDuration = duration * 0.2f;
        float elapsedTime = 0f;
        while (elapsedTime < peakDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // 3. 나머지 시간 동안 normalVValue로 점진적 변경
        float transitionDuration = duration - peakDuration;
        elapsedTime = 0f; // 타이머 리셋

        if (transitionDuration > 0)
        {
            while (elapsedTime < transitionDuration)
            {
                float t = elapsedTime / transitionDuration; // 0에서 1로
                foreach (Renderer rend in _targetedRenderers)
                {
                    if (_initialRendererColors.TryGetValue(rend, out Color initialColor))
                    {
                        Color.RGBToHSV(initialColor, out float H, out float S, out _);
                        float currentV = Mathf.Lerp(highlightVValue, normalVValue, t);
                        Color newColor = Color.HSVToRGB(H, S, currentV);

                        rend.GetPropertyBlock(_propertyBlock);
                        _propertyBlock.SetColor(mainColorPropertyName, newColor);
                        rend.SetPropertyBlock(_propertyBlock);
                    }
                }
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        // 4. 최종적으로 normalVValue 적용 (또는 원래 색상으로 복구하려면 ApplyOriginalColors())
        SetRenderersColorToVValue(normalVValue);

        _hitFlashCoroutine = null;
    }

    private void SetRenderersColorToVValue(float vValue)
    {
        if (_targetedRenderers == null || _propertyBlock == null) return;

        foreach (Renderer rend in _targetedRenderers)
        {
            if (_initialRendererColors.TryGetValue(rend, out Color initialColor))
            {
                Color.RGBToHSV(initialColor, out float H, out float S, out _);
                Color targetColor = Color.HSVToRGB(H, S, vValue);

                rend.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor(mainColorPropertyName, targetColor);
                rend.SetPropertyBlock(_propertyBlock);
            }
        }
    }

    private void ApplyOriginalColors()
    {
        if (_targetedRenderers == null || _propertyBlock == null || _initialRendererColors.Count == 0) return;

        foreach (Renderer rend in _targetedRenderers)
        {
            if (_initialRendererColors.TryGetValue(rend, out Color initialColor))
            {
                rend.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor(mainColorPropertyName, initialColor);
                rend.SetPropertyBlock(_propertyBlock);
            }
        }
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
        if (Owner.Collider != null && !Owner.Collider.enabled)
        {
            Owner.Collider.enabled = true;
        }

        Vector3 startPos = Owner.transform.position;
        Vector3 knockbackTarget = startPos + knockbackDir * Owner.TakedDamage.DamageForce;

        _airSequence?.Kill();
        _airSequence = DOTween.Sequence();
        _airSequence.Append(Owner.transform.DOMove(knockbackTarget, _knockbackTime).SetEase(Ease.OutQuad));

        float staggerDuration = (Owner.EnemyData != null && Owner.EnemyData.StaggerTime > 0) ? Owner.EnemyData.StaggerTime : hitFlashEffectDuration;
        float remainingStagger = staggerDuration - _knockbackTime;

        if (remainingStagger > 0)
        {
            _airSequence.AppendInterval(remainingStagger);
        }

        _airSequence.OnComplete(() =>
        {
            // if (Owner.NavMeshAgent != null)
            // {
            //     Owner.NavMeshAgent.enabled = true;
            //     if (Owner.NavMeshAgent.isOnNavMesh) Owner.NavMeshAgent.Warp(Owner.transform.position);
            //     Owner.NavMeshAgent.isStopped = false;
            // }
            if (SuperMachine != null) SuperMachine.ChangeState<IdleState>();
        });
    }

    public override void Update()
    {
        base.Update();
        _hitTimer += Time.deltaTime; // 일반 deltaTime 사용 (게임 시간에 따른 상태 지속시간)

        float currentStaggerTime = (Owner.EnemyData != null && Owner.EnemyData.StaggerTime > 0) ? Owner.EnemyData.StaggerTime : hitFlashEffectDuration;

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

        #region 피격머테리얼 효과
        // --- 피격 머테리얼 효과 종료 및 복구 ---
        if (_hitFlashCoroutine != null)
        {
            StopCoroutine(_hitFlashCoroutine);
            _hitFlashCoroutine = null;
            ApplyOriginalColors(); // 코루틴 강제 종료 시 원래 색상으로 복구
        }
        else
        {
            // 코루틴이 이미 완료되었거나 실행되지 않은 경우에도,
            // 만약을 위해 원래 색상으로 복구 (예: duration이 0이어서 코루틴이 안돌아간 경우)
            // 이미 CacheRenderersAndInitialColors가 호출되었다는 가정하에 _targetedRenderers 사용
            if (_targetedRenderers != null && _targetedRenderers.Count > 0 && _initialRendererColors.Count > 0)
            {
                ApplyOriginalColors();
            }
        }
        // --- 피격 머테리얼 효과 종료 및 복구 끝 ---
        #endregion 피격 머테리얼 효과


        if (Owner.NavMeshAgent != null)
        {
            if (!Owner.NavMeshAgent.enabled) Owner.NavMeshAgent.enabled = true;
            if (Owner.NavMeshAgent.isOnNavMesh) Owner.NavMeshAgent.Warp(Owner.transform.position);
            Owner.NavMeshAgent.isStopped = false;
        }

        if (Owner.Collider != null && !Owner.Collider.enabled)
        {
            if (!Owner.IsInAir)
            {
                Owner.Collider.enabled = true;
            }
        }
    }
}
