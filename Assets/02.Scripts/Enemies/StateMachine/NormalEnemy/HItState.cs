using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class HItState : BaseNormalEnemyState // BaseNormalEnemyState는 실제 프로젝트에 맞게 수정/정의되어 있어야 합니다.
{
    // 타이머 및 시퀀스
    private float _hitTimer;
    private Sequence _airSequence;

    // --- 피격 머테리얼 효과 관련 변수 ---
    private Coroutine _hitFlashCoroutine;

    [Header("Hit Flash Material Effect")] [Tooltip("효과를 적용할 머티리얼의 이름 (예: Boid)")]
    public string targetMaterialName = "KyleRobot"; // 기본값 예시, Owner.MaterialName 등으로 대체 가능

    [Tooltip("머티리얼의 메인 색상 프로퍼티 이름 (예: _BaseColor, _Color)")]
    public string mainColorPropertyName = "_Color";

    [Tooltip("피격 시 밝기(V) 값 (0.0 ~ 1.0)")] [Range(0f, 5f)] // HDR 색상을 고려하여 V값 범위를 넓힐 수 있습니다. 보통은 1.0 이상.
    public float highlightVValue = 1.5f;

    [Tooltip("피격 후 잠시 유지될 밝기(V) 값 (0.0 ~ 1.0)")] [Range(0f, 5f)]
    public float normalVValue = 0.5f; // 기본 머티리얼의 V값과 유사하게 설정

    [Tooltip("피격 효과 지속 시간 (초). StaggerTime 보다 짧게 설정될 수 있음")]
    public float hitFlashEffectDuration = 0.3f;

    private List<Renderer> _targetedRenderers;
    private MaterialPropertyBlock _propertyBlock;
    private Dictionary<Renderer, Color> _initialRendererColors = new Dictionary<Renderer, Color>();
    // --- 피격 머테리얼 효과 관련 변수 끝 ---

    // --- 넉백/공중띄우기 관련 변수 ---
    [Header("Knockback & Airborne")] [SerializeField]
    private float _maxAirHeight = 5f;

    [SerializeField] private float _airRiseAmount = 1f;
    [SerializeField] private float _airRiseTime = 0.2f;
    [SerializeField] private float _hangTime = 0.15f;
    [SerializeField] private float _fallTime = 0.5f;

    private Vector3 _knockbackDir;
    [SerializeField] private float _knockbackDistance = 2f;
    [SerializeField] private float _knockbackTime = 0.2f;
    [SerializeField] private float _knockbackAirbonCoeff = 1.5f; // 공중 넉백 시 추가 거리 계수
    // --- 넉백/공중띄우기 관련 변수 끝 ---

    // --- 공중 상태 콜라이더 제어 관련 변수 ---
    [Header("Airborne Collider Settings")]
    [Tooltip("공중에 떠 있는 동안 콜라이더가 활성화되어 있는 시간 (초). 이 시간 동안 공중 콤보 가능.")]
    [SerializeField]
    private float airborneColliderActiveDuration = 0.8f;

    private Coroutine _airborneColliderManagementCoroutine;
    private Collider _ownerCollider; // Owner의 메인 콜라이더 캐싱용
    // --- 공중 상태 콜라이더 제어 관련 변수 끝 ---

    // --- 바닥 감지 Raycast 관련 변수 ---
    [Header("Ground Detection for Landing")] [Tooltip("바닥 감지 레이캐스트 시 '바닥'으로 간주될 레이어들을 선택합니다.")] [SerializeField]
    private LayerMask groundLayerMask;

    [Tooltip("캐릭터의 현재 위치에서 레이캐스트 시작점의 Y축 오프셋입니다. (예: 0.1f 정도 위에서 시작)")] [SerializeField]
    private float raycastOriginOffsetY = 0.1f;

    [Tooltip("바닥 감지 레이캐스트의 최대 거리입니다.")] [SerializeField]
    private float groundRaycastDistance = 10f;

    [Tooltip("캐릭터가 바닥에 최종 착지할 때 Y값에 추가될 오프셋입니다. 캐릭터 피봇 위치에 따라 조정합니다. (피봇이 발바닥이면 0)")] [SerializeField]
    private float landingYOffset = 1f;

    [Tooltip("레이캐스트가 바닥을 찾지 못했을 경우 사용될 기본 착지 Y 좌표입니다.")] [SerializeField]
    private float defaultFallbackLandY = 1f;
    // --- 바닥 감지 Raycast 관련 변수 끝 ---

    // Owner와 관련된 프로퍼티 (BaseNormalEnemyState 또는 그 부모 클래스에 정의되어 있다고 가정)
    // 예시: public class BaseNormalEnemyState : MonoBehaviour {
    //          public NavMeshAgent NavMeshAgent { get; protected set; }
    //          public Animator Animator { get; protected set; }
    //          public NormalEnemyData EnemyData { get; protected set; } // InAirThreshold, StaggerTime 등
    //          public GameObject Target { get; set; }
    //          public float TakedDamageValue { get; set; }
    //          public bool IsHit { get; set; }
    //          public bool IsInAir { get; set; }
    //          public string MaterialName { get; set; } // 예시 머티리얼 이름
    //          public StateMachine SuperMachine { get; set; } // 상태 변경용
    //       }
    // 위 Owner 프로퍼티들은 실제 프로젝트의 구조에 맞게 접근해야 합니다.
    // 여기서는 this.Owner.NavMeshAgent 등으로 접근한다고 가정합니다. (Owner는 HItState의 부모클래스 멤버)

    private void CacheRenderersAndInitialColors()
    {
        // Debug.Log($"[HitState] CacheRenderersAndInitialColors: Called for {Owner?.gameObject.name}. Targeting material: '{targetMaterialName}', property: '{mainColorPropertyName}'.");
        if (Owner == null)
        {
            // Debug.LogError("[HitState] CacheRenderersAndInitialColors: Owner is NULL.");
            return;
        }

        if (string.IsNullOrEmpty(targetMaterialName))
        {
            // Debug.LogWarning("[HitState] CacheRenderersAndInitialColors: targetMaterialName is not set.");
            return;
        }

        if (_targetedRenderers == null) _targetedRenderers = new List<Renderer>();
        _targetedRenderers.Clear();

        Renderer[] allRenderers = Owner.GetComponentsInChildren<Renderer>(true);
        // Debug.Log($"[HitState] CacheRenderersAndInitialColors: Found {allRenderers.Length} total renderers under {Owner.gameObject.name}.");

        _initialRendererColors.Clear();
        bool propertyExistsOnAnyTargetMaterial = false;

        foreach (Renderer rend in allRenderers)
        {
            if (rend != null && rend.sharedMaterials != null) // sharedMaterials로 여러 머티리얼 지원
            {
                foreach (Material mat in rend.sharedMaterials)
                {
                    if (mat != null && mat.name.Contains(targetMaterialName)) // Contains로 (Instance) 등 처리
                    {
                        if (mat.HasProperty(mainColorPropertyName))
                        {
                            if (!_targetedRenderers.Contains(rend)) _targetedRenderers.Add(rend);
                            if (!_initialRendererColors.ContainsKey(rend)) // 여러 머티리얼 중 첫번째 것만 저장 (단순화)
                            {
                                _initialRendererColors[rend] = mat.GetColor(mainColorPropertyName);
                                // Debug.Log($"[HitState] CacheRenderersAndInitialColors: Targeted Renderer '{rend.gameObject.name}' (Material: '{mat.name}') HAS property '{mainColorPropertyName}'. Storing initial color: {_initialRendererColors[rend]}");
                                propertyExistsOnAnyTargetMaterial = true;
                            }
                        }

                        // else Debug.LogWarning($"[HitState] CacheRenderersAndInitialColors: Renderer '{rend.gameObject.name}' (Material: '{mat.name}') matches target name BUT DOES NOT HAVE property '{mainColorPropertyName}'. Skipping.");
                        break; // 해당 렌더러에서 일치하는 머티리얼 하나 찾으면 다음 렌더러로
                    }
                }
            }
        }

        if (_targetedRenderers.Count == 0)
        {
            // Debug.LogWarning($"[HitState] CacheRenderersAndInitialColors: NO RENDERERS found using material '{targetMaterialName}' with property '{mainColorPropertyName}'. Color effects will not work.");
        }
        else
        {
            // Debug.Log($"[HitState] CacheRenderersAndInitialColors: Added {_targetedRenderers.Count} renderers to _targetedRenderers list that use material '{targetMaterialName}' and have property '{mainColorPropertyName}'.");
        }

        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
            // Debug.Log("[HitState] CacheRenderersAndInitialColors: New MaterialPropertyBlock created.");
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _hitTimer = 0f; // 피격 타이머 초기화

        if (Owner.Animator != null) Owner.Animator.updateMode = AnimatorUpdateMode.UnscaledTime;

        // Owner의 MaterialName을 targetMaterialName으로 설정 (필요시 사용)
        // if (!string.IsNullOrEmpty(Owner.MaterialName)) targetMaterialName = Owner.MaterialName;

        // 피격 머테리얼 효과 초기화 로직 (주석 처리된 부분, 필요시 활성화)
        // CacheRenderersAndInitialColors();
        // if (_targetedRenderers != null && _targetedRenderers.Count > 0 && _initialRendererColors.Count > 0)
        // {
        //     if (_hitFlashCoroutine != null) StopCoroutine(_hitFlashCoroutine);
        //     float staggerDuration = (Owner.EnemyData != null && Owner.EnemyData.StaggerTime > 0) ? Owner.EnemyData.StaggerTime : hitFlashEffectDuration;
        //     float currentFlashDuration = Mathf.Min(staggerDuration, hitFlashEffectDuration);
        //     if(currentFlashDuration > 0) _hitFlashCoroutine = StartCoroutine(HitFlashMainColorCoroutine(currentFlashDuration));
        // }

        _ownerCollider = Owner.GetComponent<Collider>(); // Owner의 메인 콜라이더 가져오기
        if (_ownerCollider == null)
        {
            Debug.LogWarning(
                $"[HitState] OnEnter: Owner {Owner.gameObject.name} does not have a Collider component. Airborne collider management will not work effectively.");
        }

        Owner.IsHit = false; // 피격 처리 플래그 리셋
        if (Owner.NavMeshAgent != null && Owner.NavMeshAgent.isOnNavMesh && Owner.NavMeshAgent.enabled)
        {
            Owner.NavMeshAgent.isStopped = true; // 이동 중지
            Owner.NavMeshAgent.enabled = false; // NavMeshAgent 잠시 비활성화 (넉백/공중부양 중 물리적 이동 방해 방지)
        }

        // 공격자 방향으로 넉백 방향 설정
        if (Owner.Target != null) // Target이 있어야 방향 설정 가능
        {
            Vector3 direction = (Owner.transform.position - Owner.Target.transform.position).normalized;
            direction.y = 0; // Y축 방향은 제외
            _knockbackDir = direction;
        }
        else // Target이 없으면 후방으로 넉백 (예시)
        {
            _knockbackDir = -Owner.transform.forward;
        }


        // 데미지 양에 따라 공중 상태 또는 일반 피격 상태 결정
        if (Owner.EnemyData != null && Owner.TakedDamageValue >= Owner.EnemyData.InAirThreshold)
        {
            float currentY = Owner.transform.position.y;
            float desiredY = Mathf.Min(currentY + _airRiseAmount, _maxAirHeight); // 최대 높이 제한

            if (desiredY > currentY) // 실제 공중으로 뜰 수 있는 경우
            {
                Owner.IsInAir = true; // 공중 상태 시작
                if (Owner.Animator != null)
                {
                    Owner.Animator.SetFloat("DownType", Random.Range(0, 4)); // 공중 피격 애니메이션 타입 랜덤 설정
                    Owner.Animator.SetTrigger("OnDown"); // 공중 피격 애니메이션 실행
                }

                // 공중 상태 진입 시 콜라이더 관리 코루틴 시작
                if (_ownerCollider != null)
                {
                    _ownerCollider.enabled = true; // 공중으로 뜨는 순간에는 콜라이더가 있어야 함
                    if (_airborneColliderManagementCoroutine != null)
                        StopCoroutine(_airborneColliderManagementCoroutine);
                    _airborneColliderManagementCoroutine = StartCoroutine(ManageAirborneCollider());
                }

                PlayAirborneKnockbackSequence(desiredY, _knockbackDir);
            }
            else // 공중으로 뜨기엔 높이가 부족하거나 이미 최대 높이인 경우 (일반 넉백으로 처리)
            {
                Owner.IsInAir = false;
                if (Owner.Animator != null)
                {
                    Owner.Animator.SetFloat("HitType", Random.Range(1, 3)); // 일반 피격 애니메이션 타입
                    Owner.Animator.SetTrigger("OnHit"); // 일반 피격 애니메이션 실행
                }

                PlayKnockbackOnly(_knockbackDir);
            }
        }
        else // 일반 피격
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

    private IEnumerator ManageAirborneCollider()
    {
        if (_ownerCollider == null)
        {
            // Debug.LogWarning("[HitState] ManageAirborneCollider: _ownerCollider is null. Cannot manage.");
            yield break;
        }

        // 이 코루틴은 PlayAirborneKnockbackSequence에서 _ownerCollider.enabled = true 된 직후 시작됨
        // Debug.Log($"[HitState] ManageAirborneCollider: Airborne state started. Collider will be active for {airborneColliderActiveDuration}s.");

        yield return new WaitForSeconds(airborneColliderActiveDuration);

        if (Owner.IsInAir && _ownerCollider.enabled) // 아직 공중 상태이고 콜라이더가 활성화 상태일 때만 비활성화
        {
            _ownerCollider.enabled = false;
            // Debug.Log("[HitState] ManageAirborneCollider: Collider disabled after duration for landing.");
        }

        _airborneColliderManagementCoroutine = null;
    }

    private void PlayAirborneKnockbackSequence(float toY, Vector3 knockbackDir)
    {
        Vector3 startPos = Owner.transform.position;
        Vector3 knockbackMove = knockbackDir * _knockbackDistance * _knockbackAirbonCoeff;
        Vector3 riseTarget = new Vector3(startPos.x + knockbackMove.x, toY, startPos.z + knockbackMove.z);

        float finalFallY;
        RaycastHit hitInfo;
        Vector3 rayOrigin = new Vector3(riseTarget.x, Owner.transform.position.y + raycastOriginOffsetY, riseTarget.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out hitInfo, groundRaycastDistance, groundLayerMask))
        {
            finalFallY = hitInfo.point.y + landingYOffset;
            // Debug.Log($"[HitState] Raycast hit ground at Y: {hitInfo.point.y}. Landing Y set to: {finalFallY}");
        }
        else
        {
            finalFallY = defaultFallbackLandY;
            // Debug.LogWarning($"[HitState] Raycast downwards did not hit ground from {rayOrigin}. Using fallback landing Y: {finalFallY}");
        }

        Vector3 fallPos = new Vector3(riseTarget.x, finalFallY, riseTarget.z);

        _airSequence?.Kill(); // 이전 시퀀스가 있다면 중단
        _airSequence = DOTween.Sequence();
        _airSequence.Append(Owner.transform.DOMove(riseTarget, _airRiseTime).SetEase(Ease.OutSine))
            .AppendInterval(_hangTime)
            .Append(Owner.transform.DOMove(fallPos, _fallTime).SetEase(Ease.InQuad))
            .OnComplete(() =>
            {
                Owner.IsInAir = false;
                Owner.transform.position = fallPos; // 최종 위치 보정

                if (Owner.NavMeshAgent != null)
                {
                    Owner.NavMeshAgent.enabled = true; // NavMeshAgent 다시 활성화
                    // Warp는 NavMesh 위로 안전하게 이동시키지만, Y값은 NavMesh에 맞춰질 수 있음
                    if (Owner.NavMeshAgent.isOnNavMesh) Owner.NavMeshAgent.Warp(fallPos);
                    else
                        Owner.NavMeshAgent.Warp(new Vector3(fallPos.x, Owner.transform.position.y,
                            fallPos.z)); // NavMesh 밖에 있다면 현재 Y값 사용
                }

                if (SuperMachine != null) SuperMachine.ChangeState<DownedState>(); // DownedState는 실제 프로젝트에 맞게 정의
            });
    }

    private void PlayKnockbackOnly(Vector3 knockbackDir)
    {
        if (_ownerCollider != null && !_ownerCollider.enabled) // 일반 넉백 시에는 콜라이더가 켜져 있어야 함
        {
            _ownerCollider.enabled = true;
        }

        Vector3 startPos = Owner.transform.position;
        Vector3 knockbackTarget = startPos + knockbackDir * _knockbackDistance;
        knockbackTarget.y = startPos.y; // Y축 고정

        _airSequence?.Kill();
        _airSequence = DOTween.Sequence();
        _airSequence.Append(Owner.transform.DOMove(knockbackTarget, _knockbackTime).SetEase(Ease.OutQuad));

        float staggerDuration = (Owner.EnemyData != null && Owner.EnemyData.StaggerTime > 0)
            ? Owner.EnemyData.StaggerTime
            : hitFlashEffectDuration;
        float remainingStagger = staggerDuration - _knockbackTime;

        if (remainingStagger > 0)
        {
            _airSequence.AppendInterval(remainingStagger);
        }

        _airSequence.OnComplete(() =>
        {
            if (Owner.NavMeshAgent != null)
            {
                Owner.NavMeshAgent.enabled = true;
                if (Owner.NavMeshAgent.isOnNavMesh) Owner.NavMeshAgent.Warp(Owner.transform.position); // 현재 위치로 Warp
                Owner.NavMeshAgent.isStopped = false; // 이동 가능 상태로
            }

            if (SuperMachine != null) SuperMachine.ChangeState<IdleState>(); // IdleState는 실제 프로젝트에 맞게 정의
        });
    }

    public override void Update()
    {
        base.Update();
        _hitTimer += Time.deltaTime;

        // StaggerTime은 EnemyData에서 가져오거나, 없다면 피격 효과 시간(hitFlashEffectDuration)을 최소값으로 사용
        float currentStaggerTime = (Owner.EnemyData != null && Owner.EnemyData.StaggerTime > 0)
            ? Owner.EnemyData.StaggerTime
            : hitFlashEffectDuration;

        if (_hitTimer >= currentStaggerTime)
        {
            // 공중 시퀀스가 진행 중이 아니고, 공중 상태가 아닐 때만 Idle로 전환 가능
            if ((_airSequence == null || !_airSequence.IsActive()) && !Owner.IsInAir)
            {
                if (SuperMachine != null) SuperMachine.ChangeState<IdleState>();
            }
            // Owner.IsInAir 상태라면 공중 시퀀스가 완료되거나 DownedState로 전환될 때까지 대기
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        if (Owner.Animator != null) Owner.Animator.updateMode = AnimatorUpdateMode.Normal;

        _airSequence?.Kill();
        _hitTimer = 0f;

        if (_hitFlashCoroutine != null)
        {
            StopCoroutine(_hitFlashCoroutine);
            _hitFlashCoroutine = null;
        }
        // ApplyOriginalColors(); // 원본 색상으로 복구하는 함수 (필요시 구현 및 호출)

        if (_airborneColliderManagementCoroutine != null)
        {
            StopCoroutine(_airborneColliderManagementCoroutine);
            _airborneColliderManagementCoroutine = null;
            // Debug.Log("[HitState] OnExit: Stopped airborne collider coroutine.");
        }

        // NavMeshAgent 활성화 및 상태 복구
        if (Owner.NavMeshAgent != null)
        {
            if (!Owner.NavMeshAgent.enabled) Owner.NavMeshAgent.enabled = true;
            if (Owner.NavMeshAgent.isOnNavMesh) Owner.NavMeshAgent.Warp(Owner.transform.position);
            Owner.NavMeshAgent.isStopped = false;
        }

        // 콜라이더 상태 복구: 공중 상태가 아니면서 콜라이더가 꺼져있다면 다시 켬
        // (DownedState 같은 특정 상태에서 의도적으로 꺼둘 경우 해당 상태의 OnEnter에서 다시 관리)
        if (_ownerCollider != null && !_ownerCollider.enabled)
        {
            if (!Owner.IsInAir) // 공중 상태가 아니라면 (즉, 착지했거나 일반 피격 후 종료)
            {
                _ownerCollider.enabled = true;
                // Debug.Log("[HitState] OnExit: Collider re-enabled as Owner is not in air.");
            }
            // 만약 Owner.IsInAir가 true인 채로 HitState가 종료된다면 (예: 다른 상태로 즉시 전환),
            // 다음 상태에서 콜라이더를 적절히 처리해야 함. (예: DeadState에서 Ragdoll 처리 등)
        }
    }
}