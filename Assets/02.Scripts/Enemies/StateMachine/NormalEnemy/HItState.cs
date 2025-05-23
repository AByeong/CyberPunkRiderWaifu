using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class HItState : BaseNormalEnemyState
{
    private float _hitTimer;
    private Sequence _airSequence;

    // --- 피격 머테리얼 효과 관련 변수 ---
    private Coroutine _hitFlashCoroutine;

    [Header("Hit Flash Material Effect")]
    [Tooltip("효과를 적용할 머티리얼의 이름 (예: Boid)")]
    public string targetMaterialName = "KyleRobot"; // 특정 머티리얼 이름 지정

    [Tooltip("머티리얼의 메인 색상 프로퍼티 이름 (예: _BaseColor, _Color)")]
    public string mainColorPropertyName = "_Color";

    [Tooltip("피격 시 밝기(V) 값 (0.0 ~ 1.0)")]
    public float highlightVValue = 1.0f;

    [Tooltip("피격 후 잠시 유지될 밝기(V) 값 (0.0 ~ 1.0)")]
    public float normalVValue = 0.5f;

    [Tooltip("피격 효과 지속 시간 (초). StaggerTime 보다 짧게 설정될 수 있음")]
    public float hitFlashEffectDuration = 0.3f; // 이전 defaultHitFlashDuration에서 이름 변경 및 역할 명확화

    private List<Renderer> _targetedRenderers; // 필터링된 렌더러 목록
    private MaterialPropertyBlock _propertyBlock;
    private Dictionary<Renderer, Color> _initialRendererColors = new Dictionary<Renderer, Color>();
    // --- 피격 머테리얼 효과 관련 변수 끝 ---

    // (넉백/공중띄우기 관련 변수들은 그대로 유지)
    private float _maxAirHeight = 5f;
    private float _airRiseAmount = 1f;
    private float _airRiseTime = 0.2f;
    private float _hangTime = 0.15f;
    private float _fallTime = 0.5f;

    private Vector3 _knockbackDir;
    private float _knockbackDistance = 2f;
    private float _knockbackTime = 0.2f;
    private float _knockbackAirbonCoeff = 1.5f;


    private void CacheRenderersAndInitialColors()
    {
        Debug.Log($"[HitState] CacheRenderersAndInitialColors: Called for {Owner?.gameObject.name}. Targeting material: '{targetMaterialName}', property: '{mainColorPropertyName}'.");
        if (Owner == null)
        {
            Debug.LogError("[HitState] CacheRenderersAndInitialColors: Owner is NULL.");
            return;
        }

        if (_targetedRenderers == null)
        {
            _targetedRenderers = new List<Renderer>();
        }
        _targetedRenderers.Clear(); // 매번 새로 필터링된 렌더러 목록 생성

        List<Renderer> allRenderers = new List<Renderer>();
        Owner.GetComponentsInChildren<Renderer>(true, allRenderers); // 모든 자식 렌더러 일단 가져오기
        Debug.Log($"[HitState] CacheRenderersAndInitialColors: Found {allRenderers.Count} total renderers under {Owner.gameObject.name}.");

        _initialRendererColors.Clear();
        bool propertyExistsOnAnyTargetMaterial = false;

        for (int i = 0; i < allRenderers.Count; i++)
        {
            Renderer rend = allRenderers[i];
            if (rend != null && rend.sharedMaterial != null)
            {
                // 1. 머티리얼 이름 필터링
                if (rend.sharedMaterial.name == targetMaterialName) // 인스턴스 머티리얼 이름은 (Instance)가 붙을 수 있으니 주의
                {
                    // 2. 프로퍼티 존재 확인
                    if (rend.sharedMaterial.HasProperty(mainColorPropertyName))
                    {
                        _targetedRenderers.Add(rend); // 필터링된 렌더러 목록에 추가
                        _initialRendererColors[rend] = rend.sharedMaterial.GetColor(mainColorPropertyName);
                        if (!propertyExistsOnAnyTargetMaterial)
                        {
                             Debug.Log($"[HitState] CacheRenderersAndInitialColors: Targeted Renderer '{rend.gameObject.name}' (Material: '{rend.sharedMaterial.name}') HAS property '{mainColorPropertyName}'. Storing initial color: {_initialRendererColors[rend]}");
                             propertyExistsOnAnyTargetMaterial = true;
                        }
                    }
                    else
                    {
                         Debug.LogWarning($"[HitState] CacheRenderersAndInitialColors: Renderer '{rend.gameObject.name}' (Material: '{rend.sharedMaterial.name}') matches target name BUT DOES NOT HAVE property '{mainColorPropertyName}'. Skipping.");
                    }
                }
                // else: 머티리얼 이름이 다르면 스킵
            }
        }

        if (_targetedRenderers.Count == 0) {
            Debug.LogWarning($"[HitState] CacheRenderersAndInitialColors: NO RENDERERS found using material '{targetMaterialName}' with property '{mainColorPropertyName}'. Color effects will not work.");
        } else {
            Debug.Log($"[HitState] CacheRenderersAndInitialColors: Added {_targetedRenderers.Count} renderers to _targetedRenderers list that use material '{targetMaterialName}' and have property '{mainColorPropertyName}'.");
        }


        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
            Debug.Log("[HitState] CacheRenderersAndInitialColors: New MaterialPropertyBlock created.");
        }
    }

    public override void OnEnter()
    {

        Owner.gameObject.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        
        targetMaterialName = Owner.MaterialName;
        
        
        Debug.Log($"[HitState] OnEnter: Called for {Owner?.gameObject.name}");
        _airSequence?.Kill();
        base.OnEnter();

        CacheRenderersAndInitialColors();

        Debug.Log("[HitState] OnEnter: Attempting to start hit flash effect.");
        if (_targetedRenderers != null && _targetedRenderers.Count > 0 && _initialRendererColors.Count > 0) // _targetedRenderers로 조건 변경
        {
            if (_hitFlashCoroutine != null)
            {
                Debug.Log("[HitState] OnEnter: Stopping existing HitFlashCoroutine.");
                StopCoroutine(_hitFlashCoroutine);
            }

            float currentFlashDuration = hitFlashEffectDuration; // 인스펙터에서 설정한 값을 기본으로 사용
            if (Owner.EnemyData != null && Owner.EnemyData.StaggerTime > 0)
            {
                // StaggerTime이 효과 지속시간보다 짧으면 StaggerTime을 따르고, 아니면 설정된 효과 지속시간 사용
                currentFlashDuration = Mathf.Min(Owner.EnemyData.StaggerTime, hitFlashEffectDuration);
                 Debug.Log($"[HitState] OnEnter: Flash duration will be {currentFlashDuration} (Min of StaggerTime and hitFlashEffectDuration).");
            }
            else
            {
                 Debug.Log($"[HitState] OnEnter: Using hitFlashEffectDuration = {currentFlashDuration}");
            }

            if(currentFlashDuration <= 0) Debug.LogWarning($"[HitState] OnEnter: currentFlashDuration ({currentFlashDuration}) is zero or negative.");
            _hitFlashCoroutine = StartCoroutine(HitFlashMainColorCoroutine(currentFlashDuration));
        }
        else
        {
            Debug.LogWarning($"[HitState] OnEnter: No targeted renderers to apply hit flash effect. Check targetMaterialName ('{targetMaterialName}') and mainColorPropertyName ('{mainColorPropertyName}').");
        }

        Owner.IsHit = false;
        if (Owner.NavMeshAgent != null && Owner.NavMeshAgent.isOnNavMesh)
        {
            Owner.NavMeshAgent.enabled = false;
        }

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
            PlayKnockbackOnly(_knockbackDir);
        }
    }

    private IEnumerator HitFlashMainColorCoroutine(float duration)
    {
        Debug.Log($"[HitState] HitFlashMainColorCoroutine: Started. Duration: {duration}. Target Highlight V: {highlightVValue}");
        ApplyMainColorHSVModification(highlightVValue);
        Debug.Log("[HitState] HitFlashMainColorCoroutine: Highlight V value applied.");

        if (duration > 0) {
            yield return new WaitForSeconds(duration);
            Debug.Log("[HitState] HitFlashMainColorCoroutine: Wait finished.");
        }

        Debug.Log($"[HitState] HitFlashMainColorCoroutine: Setting Normal V value ({normalVValue}).");
        ApplyMainColorHSVModification(normalVValue);
        Debug.Log("[HitState] HitFlashMainColorCoroutine: Normal V value applied.");

        _hitFlashCoroutine = null;
        Debug.Log("[HitState] HitFlashMainColorCoroutine: Coroutine finished.");
    }

    private void ApplyMainColorHSVModification(float targetV)
    {
        Debug.Log($"[HitState] ApplyMainColorHSVModification: Called with targetV: {targetV} for property '{mainColorPropertyName}'. Processing {_targetedRenderers?.Count} targeted renderers.");
        if (_targetedRenderers == null || _propertyBlock == null || _initialRendererColors == null) return;

        int appliedCount = 0;
        foreach (Renderer rend in _targetedRenderers) // _targetedRenderers 사용
        {
            if (rend != null) // rend가 null이 아니고, _initialRendererColors에 있는지 확인 (Cache 로직에서 이미 필터링됨)
            {
                if (_initialRendererColors.TryGetValue(rend, out Color initialColor))
                {
                    float H, S, V_original;
                    Color.RGBToHSV(initialColor, out H, out S, out V_original);

                    Color newRGBColor = Color.HSVToRGB(H, S, Mathf.Clamp01(targetV));
                    newRGBColor.a = initialColor.a;

                    rend.GetPropertyBlock(_propertyBlock);
                    _propertyBlock.SetColor(mainColorPropertyName, newRGBColor);
                    rend.SetPropertyBlock(_propertyBlock);
                    appliedCount++;
                    if(appliedCount==1) Debug.Log($"[HitState] ApplyMainColorHSVModification: Applied to '{rend.gameObject.name}'. InitialColor: {initialColor} (H:{H:F2} S:{S:F2} V:{V_original:F2}), TargetV: {targetV}, NewRGB: {newRGBColor}");
                }
            }
        }
        Debug.Log($"[HitState] ApplyMainColorHSVModification: Finished. Applied to {appliedCount} renderers.");
    }

    private void ApplyOriginalColors()
    {
        Debug.Log($"[HitState] ApplyOriginalColors: Reverting to initial colors for property '{mainColorPropertyName}'. Processing {_targetedRenderers?.Count} targeted renderers.");
        if (_targetedRenderers == null || _propertyBlock == null || _initialRendererColors == null) return;

        int revertedCount = 0;
        foreach (Renderer rend in _targetedRenderers) // _targetedRenderers 사용
        {
            if (rend != null)
            {
                if (_initialRendererColors.TryGetValue(rend, out Color originalColor))
                {
                    rend.GetPropertyBlock(_propertyBlock);
                    _propertyBlock.SetColor(mainColorPropertyName, originalColor);
                    rend.SetPropertyBlock(_propertyBlock);
                    revertedCount++;
                    if(revertedCount==1) Debug.Log($"[HitState] ApplyOriginalColors: Reverted '{rend.gameObject.name}' to {originalColor}.");
                }
            }
        }
        Debug.Log($"[HitState] ApplyOriginalColors: Finished. Reverted {revertedCount} renderers.");
    }

    private void PlayAirborneKnockbackSequence(float toY, Vector3 knockbackDir)
    {
        Vector3 startPos = Owner.transform.position;
        Vector3 knockbackMove = knockbackDir * _knockbackDistance * _knockbackAirbonCoeff;
        Vector3 riseTarget = new Vector3(startPos.x + knockbackMove.x, toY, startPos.z + knockbackMove.z);
        Vector3 fallPos = new Vector3(riseTarget.x, 1.7f, riseTarget.z);

        _airSequence = DOTween.Sequence();
        _airSequence.Append(Owner.transform.DOMove(riseTarget, _airRiseTime).SetEase(Ease.OutSine))
                    .AppendInterval(_hangTime)
                    .Append(Owner.transform.DOMoveY(fallPos.y, _fallTime).SetEase(Ease.InQuad))
                    .OnComplete(() =>
                    {
                        Owner.IsInAir = false;
                        if (Owner.NavMeshAgent != null)
                        {
                            Owner.NavMeshAgent.enabled = true;
                            if(Owner.NavMeshAgent.isOnNavMesh) Owner.NavMeshAgent.Warp(Owner.transform.position);
                        }
                        SuperMachine.ChangeState<DownedState>();
                    });
    }

    private void PlayKnockbackOnly(Vector3 knockbackDir)
    {
        Vector3 startPos = Owner.transform.position;
        Vector3 knockbackTarget = startPos + knockbackDir * _knockbackDistance;
        knockbackTarget.y = startPos.y;

        _airSequence = DOTween.Sequence();
        _airSequence.Append(Owner.transform.DOMove(knockbackTarget, _knockbackTime).SetEase(Ease.OutQuad));

        float tempStaggerFallback = (Owner.EnemyData != null && Owner.EnemyData.StaggerTime > 0) ? Owner.EnemyData.StaggerTime : hitFlashEffectDuration;
        float remainingStagger = tempStaggerFallback - _knockbackTime;

        if (remainingStagger > 0)
        {
            _airSequence.AppendInterval(remainingStagger);
        }

        _airSequence.OnComplete(() =>
                    {
                        if (Owner.NavMeshAgent != null)
                        {
                            Owner.NavMeshAgent.enabled = true;
                            if(Owner.NavMeshAgent.isOnNavMesh) Owner.NavMeshAgent.Warp(Owner.transform.position);
                        }
                        SuperMachine.ChangeState<IdleState>();
                    });
    }

    public override void Update()
    {
        base.Update();
        _hitTimer += Time.deltaTime;
        float staggerTime = (Owner.EnemyData != null && Owner.EnemyData.StaggerTime > 0) ? Owner.EnemyData.StaggerTime : hitFlashEffectDuration;


        if (_hitTimer >= staggerTime)
        {
            if (_airSequence == null || !_airSequence.IsActive())
            {
                 if (!Owner.IsInAir)
                {
                    SuperMachine.ChangeState<IdleState>();
                }
            }
            return;
        }
    }

    public override void OnExit()
    {
        Owner.gameObject.GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;
        Debug.Log($"[HitState] OnExit: Called for {Owner?.gameObject.name}");
        base.OnExit();
        _airSequence?.Kill();
        _hitTimer = 0f;

        if (_hitFlashCoroutine != null)
        {
            Debug.Log("[HitState] OnExit: Stopping active HitFlashCoroutine.");
            StopCoroutine(_hitFlashCoroutine);
            _hitFlashCoroutine = null;
        }
        Debug.Log("[HitState] OnExit: Reverting all targeted materials to their initial original colors.");
        ApplyOriginalColors();


        if (Owner.NavMeshAgent != null && !Owner.NavMeshAgent.enabled)
        {
            Owner.NavMeshAgent.enabled = true;
            if(Owner.NavMeshAgent.isOnNavMesh) Owner.NavMeshAgent.Warp(Owner.transform.position);
        }
    }
}