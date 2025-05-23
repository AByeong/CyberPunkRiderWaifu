using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using System.Collections; // 코루틴 사용을 위해 추가
using System.Collections.Generic; // List 사용을 위해 추가

public class HItState : BaseNormalEnemyState
{
    private float _hitTimer;
    private Sequence _airSequence;

    // --- 피격 머테리얼 효과 관련 변수 ---
    private Coroutine _hitFlashCoroutine;
    // Shader에서 사용할 프로퍼티 이름 (float 타입이어야 함)
    public string hitFlashPropertyName = "_HitFlashIntensity"; // 예시 이름, 실제 셰이더 프로퍼티 이름으로 변경
    public float hitFlashHighlightValue = 1.0f; // 피격 시 설정될 값
    public float hitFlashNormalValue = 0.0f;    // 평소 값 (효과 없음)
    public float defaultHitFlashDuration = 0.3f; // EnemyData.StaggerTime을 사용할 수 없을 경우의 기본 지속시간

    private List<Renderer> _cachedRenderers; // 성능을 위해 렌더러 목록 캐싱
    private MaterialPropertyBlock _propertyBlock; // 머테리얼 직접 변경 대신 사용 (성능 및 인스턴싱 방지)
    // --- 피격 머테리얼 효과 관련 변수 끝 ---


    // 이 값들은 EnemyData 또는 다른 설정 파일에서 가져오는 것이 좋습니다.
    private float _maxAirHeight = 5f;
    private float _airRiseAmount = 1f;
    private float _airRiseTime = 0.2f;
    private float _hangTime = 0.15f;
    private float _fallTime = 0.5f;

    private Vector3 _knockbackDir;
    private float _knockbackDistance = 2f;     // 예시: 실제 값은 Owner.EnemyData 등에서 로드하세요.
    private float _knockbackTime = 0.2f;        // 예시: 실제 값은 Owner.EnemyData 등에서 로드하세요.
    private float _knockbackAirbonCoeff = 1.5f; // 예시: 실제 값은 Owner.EnemyData 등에서 로드하세요.


    // BaseNormalEnemyState에 OnInitialize 같은 초기화 메서드가 있다면 그곳에서 캐싱하는 것이 더 좋습니다.
    // public override void OnInitialize()
    // {
    //     base.OnInitialize();
    //     CacheRenderers();
    //     if (_propertyBlock == null)
    //     {
    //         _propertyBlock = new MaterialPropertyBlock();
    //     }
    // }

    private void CacheRenderers()
    {
        if (_cachedRenderers == null && Owner != null)
        {
            _cachedRenderers = new List<Renderer>();
            Owner.GetComponentsInChildren<Renderer>(true, _cachedRenderers); // 비활성 포함 모든 자식 렌더러 가져오기
            if (_cachedRenderers.Count == 0)
            {
                Debug.LogWarning($"No renderers found on or under {Owner.gameObject.name} for hit flash effect.", Owner.gameObject);
            }
        }
        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }
    }


    public override void OnEnter()
    {
        _airSequence?.Kill();
        base.OnEnter();

        // 렌더러 캐싱 및 MaterialPropertyBlock 초기화 (OnInitialize에서 안했다면)
        CacheRenderers();

        // --- 피격 머테리얼 효과 시작 ---
        if (_cachedRenderers != null && _cachedRenderers.Count > 0)
        {
            if (_hitFlashCoroutine != null)
            {
                StopCoroutine(_hitFlashCoroutine);
            }
            float currentHitFlashDuration = defaultHitFlashDuration;
            if (Owner.EnemyData != null && Owner.EnemyData.StaggerTime > 0)
            {
                // StaggerTime의 일부 또는 전체를 효과 지속시간으로 사용 가능
                currentHitFlashDuration = Mathf.Min(Owner.EnemyData.StaggerTime, defaultHitFlashDuration); // 예: 둘 중 짧은 시간
            }
            _hitFlashCoroutine = StartCoroutine(HitFlashCoroutine(currentHitFlashDuration));
        }
        // --- 피격 머테리얼 효과 시작 끝 ---

        Owner.IsHit = false;
        if (Owner.NavMeshAgent != null && Owner.NavMeshAgent.isOnNavMesh)
        {
            Owner.NavMeshAgent.enabled = false;
        }

        // 넉백 및 공중띄우기 관련 파라미터 로드 (Owner.EnemyData에서 가져오는 것을 권장)
        // LoadKnockbackParameters(); // 예시 메서드 호출

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
            // PlayKnockbackOnly 내부에서 애니메이션 처리하도록 수정 권장
             Owner.Animator.SetFloat("HitType", Random.Range(1, 3));
             Owner.Animator.SetTrigger("OnHit");
            PlayKnockbackOnly(_knockbackDir);
        }
    }

    private IEnumerator HitFlashCoroutine(float duration)
    {
        // 하이라이트 값으로 설정
        SetMaterialPropertyValue(hitFlashHighlightValue);

        yield return new WaitForSeconds(duration);

        // 일반 값으로 복원
        SetMaterialPropertyValue(hitFlashNormalValue);

        _hitFlashCoroutine = null;
    }

    private void SetMaterialPropertyValue(float value)
    {
        if (_cachedRenderers == null || _propertyBlock == null) return;

        for (int i = 0; i < _cachedRenderers.Count; i++)
        {
            Renderer rend = _cachedRenderers[i];
            if (rend != null) // 렌더러가 파괴되지 않았는지 확인
            {
                // 각 렌더러에 대해 MaterialPropertyBlock을 설정합니다.
                // 이렇게 하면 공유 머티리얼을 직접 변경하거나 인스턴스를 만들지 않습니다.
                rend.GetPropertyBlock(_propertyBlock); // 현재 블록 상태 가져오기 (다른 프로퍼티 유지)
                _propertyBlock.SetFloat(hitFlashPropertyName, value);
                rend.SetPropertyBlock(_propertyBlock);
            }
        }
    }

    private void PlayAirborneKnockbackSequence(float toY, Vector3 knockbackDir)
    {
        // Owner.SetRigidbodyKinematic(true); // Rigidbody 사용 시

        Vector3 startPos = Owner.transform.position;
        Vector3 knockbackMove = knockbackDir * _knockbackDistance * _knockbackAirbonCoeff;
        Vector3 riseTarget = new Vector3(startPos.x + knockbackMove.x, toY, startPos.z + knockbackMove.z);
        Vector3 fallPos = new Vector3(riseTarget.x, 1.7f, riseTarget.z);  // TODO : RAY로 지면 높이 확인하기

        _airSequence = DOTween.Sequence();
        _airSequence.Append(Owner.transform.DOMove(riseTarget, _airRiseTime).SetEase(Ease.OutSine))
                    .AppendInterval(_hangTime)
                    .Append(Owner.transform.DOMoveY(fallPos.y, _fallTime).SetEase(Ease.InQuad))
                    .OnComplete(() =>
                    {
                        Owner.IsInAir = false;
                        // Owner.SetRigidbodyKinematic(false);
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
        // Owner.SetRigidbodyKinematic(true);
        Vector3 startPos = Owner.transform.position;
        Vector3 knockbackTarget = startPos + knockbackDir * _knockbackDistance;
        knockbackTarget.y = startPos.y;

        // 애니메이션 트리거는 이 메서드 내부 또는 호출 직전이 더 적절
        // Owner.Animator.SetFloat("HitType", Random.Range(1, 3)); // 이미 OnEnter에서 호출됨
        // Owner.Animator.SetTrigger("OnHit");                      // 이미 OnEnter에서 호출됨

        _airSequence = DOTween.Sequence();
        _airSequence.Append(Owner.transform.DOMove(knockbackTarget, _knockbackTime).SetEase(Ease.OutQuad));

        float remainingStagger = (Owner.EnemyData != null ? Owner.EnemyData.StaggerTime : defaultHitFlashDuration) - _knockbackTime;
        if (remainingStagger > 0)
        {
            _airSequence.AppendInterval(remainingStagger);
        }

        _airSequence.OnComplete(() =>
                    {
                        // Owner.SetRigidbodyKinematic(false);
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
        float staggerTime = (Owner.EnemyData != null ? Owner.EnemyData.StaggerTime : defaultHitFlashDuration);

        if (_hitTimer >= staggerTime)
        {
            if (_airSequence == null || !_airSequence.IsActive())
            {
                 if (!Owner.IsInAir)
                {
                    SuperMachine.ChangeState<IdleState>();
                }
                // 공중 상태에서 DownedState로의 전환은 PlayAirborneKnockbackSequence의 OnComplete에서 처리됨
            }
            return;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        _airSequence?.Kill();
        _hitTimer = 0f;

        // 피격 효과 코루틴 중지 및 머테리얼 복원
        if (_hitFlashCoroutine != null)
        {
            StopCoroutine(_hitFlashCoroutine);
            SetMaterialPropertyValue(hitFlashNormalValue); // 상태 나갈 때 즉시 복원
            _hitFlashCoroutine = null;
        }

        if (Owner.NavMeshAgent != null && !Owner.NavMeshAgent.enabled)
        {
            Owner.NavMeshAgent.enabled = true;
            if(Owner.NavMeshAgent.isOnNavMesh) Owner.NavMeshAgent.Warp(Owner.transform.position);
        }
        // Owner.SetRigidbodyKinematic(false);
    }
}