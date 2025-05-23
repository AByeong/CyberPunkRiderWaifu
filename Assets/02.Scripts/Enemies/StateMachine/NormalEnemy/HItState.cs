using DG.Tweening;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent 사용을 위해 추가 (원본 코드에는 없었지만 일반적으로 필요)

// LODMaterialController가 특정 네임스페이스에 있다면 using 지시문 추가
// 예: using MyGame.Effects;

public class HitState : BaseNormalEnemyState
{
    private float _hitTimer;
    // private float _hitColor = 0; // LODMaterialController가 이 역할을 하므로 주석 처리 또는 삭제 가능
    private Sequence _airSequence;

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

    // LODMaterialController 참조 추가
    private LODMaterialController _lodMaterialController;

    // StateMachine의 BaseNormalEnemyState에 OnInitialize 같은 초기화 메서드가 있다면 그곳에서 참조를 할당하는 것이 좋습니다.
    // public override void OnInitialize()
    // {
    //     base.OnInitialize();
    //     // Owner는 BaseNormalEnemyState에서 접근 가능하다고 가정
    //     if (Owner != null)
    //     {
    //         _lodMaterialController = Owner.GetComponentInChildren<LODMaterialController>(true); // 자식 오브젝트에서 탐색 (비활성화 포함)
    //         if (_lodMaterialController == null)
    //         {
    //             Debug.LogWarning($"LODMaterialController not found on or under {Owner.gameObject.name}. Hit material effect will not be applied.", Owner.gameObject);
    //         }
    //     }
    // }

    public override void OnEnter()
    {
        // 이전 트윈 시퀀스가 실행 중일 수 있으므로 안전하게 Kill 처리
        _airSequence?.Kill();
        base.OnEnter();

        // LODMaterialController 참조 얻기 (OnInitialize에서 처리하지 않은 경우 또는 안전장치)
        if (_lodMaterialController == null && Owner != null)
        {
            // 일반적으로 캐릭터의 시각적 표현을 담당하는 자식 GameObject에 LODGroup과 LODMaterialController가 있을 수 있습니다.
            // 또는 Owner GameObject 자체에 있을 수도 있습니다. 상황에 맞게 GetComponent 또는 GetComponentInChildren 사용.
            _lodMaterialController = Owner.GetComponentInChildren<LODMaterialController>(true);
            if (_lodMaterialController == null)
            {
                Debug.LogWarning($"LODMaterialController not found on or under {Owner.gameObject.name} during OnEnter. Hit material effect will not be applied.", Owner.gameObject);
            }
        }

        // --- LOD 머테리얼 값 변경 트리거 ---
        if (_lodMaterialController != null)
        {
            // LODMaterialController에 설정된 기본 지속 시간을 사용하거나, 특정 시간을 지정할 수 있습니다.
            // 예시: StaggerTime 동안 또는 StaggerTime의 일부 동안 효과 적용
            // _lodMaterialController.TriggerHighlight(); // 기본 지속 시간 사용 (LODMaterialController의 TriggerHighlight 메서드 이름 및 파라미터에 따라 다름)
            if (Owner.EnemyData != null && Owner.EnemyData.StaggerTime > 0)
            {
                 // LODMaterialController의 메서드 이름이 TriggerHighlight(float duration)이라고 가정
                _lodMaterialController.TriggerHighlight(Owner.EnemyData.StaggerTime);
            }
            else
            {
                _lodMaterialController.TriggerHighlight(); // EnemyData.StaggerTime을 사용할 수 없다면 기본값 사용
            }
        }
        // --- LOD 머테리얼 값 변경 트리거 끝 ---

        Owner.IsHit = false; // 데미지 처리 후 IsHit 플래그는 외부에서 관리하거나 여기서 리셋
        if (Owner.NavMeshAgent != null) // NavMeshAgent가 null이 아닌지 확인
        {
            if(Owner.NavMeshAgent.isOnNavMesh) // NavMesh 위에 있을 때만 비활성화 시도
            {
                Owner.NavMeshAgent.enabled = false;
            }
        }


        // 넉백 및 공중띄우기 관련 파라미터 로드 (Owner.EnemyData에서 가져오는 것을 권장)
        // 예시: 다음 값들은 실제로는 Owner.EnemyData에서 가져와야 합니다.
        // _maxAirHeight = Owner.EnemyData.MaxAirHeight;
        // _airRiseAmount = Owner.EnemyData.AirRiseAmount;
        // ... (다른 파라미터들도 마찬가지) ...
        // _knockbackDistance = Owner.EnemyData.KnockbackDistance;
        // _knockbackTime = Owner.EnemyData.KnockbackTime;
        // _knockbackAirbonCoeff = Owner.EnemyData.KnockbackAirborneCoefficient;


        Vector3 direction = (Owner.transform.position - Owner.Target.transform.position).normalized;
        direction.y = 0; // 수평 방향으로만 넉백
        _knockbackDir = direction;

        if (Owner.TakedDamageValue >= Owner.EnemyData.InAirThreshold)
        {
            float currentY = Owner.transform.position.y;
            float desiredY = Mathf.Min(currentY + _airRiseAmount, _maxAirHeight);

            if (desiredY > currentY) // 실제 공중에 뜰 수 있는지 확인
            {
                Owner.IsInAir = true;
                Owner.Animator.SetFloat("DownType", Random.Range(0, 4)); // 0,1,2,3 중 랜덤
                Owner.Animator.SetTrigger("OnDown");
                PlayAirborneKnockbackSequence(desiredY, _knockbackDir);
            }
            else
            {
                // 최대 높이에 도달했거나 더 띄울 수 없으면 일반 넉백
                PlayKnockbackOnly(_knockbackDir);
            }
        }
        else // 일반 피격
        {
            // PlayKnockbackOnly 메서드 내에서 Animator.SetTrigger("OnHit") 등을 호출해야 합니다.
            // 현재 코드에서는 PlayKnockbackOnly를 호출하기 전에 애니메이션을 설정하고 있습니다.
            // 일관성을 위해 PlayKnockbackOnly 내부 또는 직전에 애니메이션 트리거를 두는 것이 좋습니다.
            // Owner.Animator.SetFloat("HitType", Random.Range(1, 3)); // 1,2 중 랜덤
            // Owner.Animator.SetTrigger("OnHit");
            PlayKnockbackOnly(_knockbackDir);
        }
    }

    private void PlayAirborneKnockbackSequence(float toY, Vector3 knockbackDir)
    {
        // Owner의 Rigidbody가 있다면 isKinematic = true로 설정하는 것이 좋습니다.
        // Owner.SetRigidbodyKinematic(true);

        Vector3 startPos = Owner.transform.position;
        Vector3 knockbackMove = knockbackDir * _knockbackDistance * _knockbackAirbonCoeff;
        Vector3 riseTarget = new Vector3(startPos.x + knockbackMove.x, toY, startPos.z + knockbackMove.z);

        // 착지 지점 Y 계산 (Raycast를 사용하는 것이 정확합니다)
        // Vector3 fallPos = new Vector3(riseTarget.x, FindGroundY(riseTarget), riseTarget.z);
        Vector3 fallPos = new Vector3(riseTarget.x, 1.7f, riseTarget.z);  // TODO : RAY로 지면 높이 확인하기 (주석 유지)

        _airSequence = DOTween.Sequence();
        _airSequence.Append(Owner.transform.DOMove(riseTarget, _airRiseTime).SetEase(Ease.OutSine)) // 띄우기 + 밀기
                    .AppendInterval(_hangTime)
                    .Append(Owner.transform.DOMoveY(fallPos.y, _fallTime).SetEase(Ease.InQuad)) // Y축만 변경하여 낙하
                    // XZ축은 이미 knockbackTarget으로 이동했으므로, 낙하 시 XZ를 다시 DOMove할 필요는 없을 수 있습니다.
                    // 만약 낙하 중에도 수평 이동이 필요하다면 .Join(Owner.transform.DOMoveX/Z(...)) 추가
                    .OnComplete(() =>
                    {
                        Owner.IsInAir = false;
                        // Owner.SetRigidbodyKinematic(false);
                        if (Owner.NavMeshAgent != null)
                        {
                           if(!Owner.NavMeshAgent.isOnNavMesh) // 워프 전에 NavMesh 위에 있는지 확인하거나, 안전하게 위치 설정
                           {
                                // Owner.transform.position = fallPos; // 위치 보정
                           }
                            Owner.NavMeshAgent.enabled = true;
                            Owner.NavMeshAgent.Warp(Owner.transform.position); // 현재 위치로 NavMeshAgent 강제 이동
                        }
                        SuperMachine.ChangeState<DownedState>();
                    });
    }

    private void PlayKnockbackOnly(Vector3 knockbackDir)
    {
        // Owner.SetRigidbodyKinematic(true);
        Vector3 startPos = Owner.transform.position;
        Vector3 knockbackTarget = startPos + knockbackDir * _knockbackDistance;
        knockbackTarget.y = startPos.y; // Y축은 고정 (지상 넉백)

        // 애니메이션 트리거는 이 메서드 내부 또는 호출 직전에 일관성 있게 위치시키는 것이 좋습니다.
        Owner.Animator.SetFloat("HitType", Random.Range(1, 3)); // 1,2 중 랜덤
        Owner.Animator.SetTrigger("OnHit");

        _airSequence = DOTween.Sequence();
        _airSequence.Append(Owner.transform.DOMove(knockbackTarget, _knockbackTime).SetEase(Ease.OutQuad));

        // StaggerTime에서 넉백 시간을 제외한 만큼 추가 대기
        float remainingStagger = Owner.EnemyData.StaggerTime - _knockbackTime;
        if (remainingStagger > 0)
        {
            _airSequence.AppendInterval(remainingStagger);
        }
        else
        {
            // StaggerTime이 _knockbackTime보다 짧거나 같으면, DOMove 직후 완료되도록 처리
            // 또는 최소한의 인터벌을 둘 수 있습니다.
            // Debug.LogWarning("StaggerTime is shorter than or equal to knockbackTime. Hit reaction might be too short.");
        }

        _airSequence.OnComplete(() =>
                    {
                        // Owner.SetRigidbodyKinematic(false);
                        if (Owner.NavMeshAgent != null)
                        {
                            Owner.NavMeshAgent.enabled = true;
                            Owner.NavMeshAgent.Warp(Owner.transform.position);
                        }
                        SuperMachine.ChangeState<IdleState>();
                    });
    }

    public override void Update()
    {
        base.Update();

        _hitTimer += Time.deltaTime;
        // DOTween 시퀀스의 OnComplete 콜백이 주로 상태 전환을 담당해야 합니다.
        // 이 타이머는 백업 또는 다른 예외 상황 처리에 사용될 수 있습니다.
        // 현재 PlayKnockbackOnly의 길이는 StaggerTime을 고려하고, PlayAirborneKnockbackSequence는 자체 길이 후 DownedState로 갑니다.
        if (Owner.EnemyData != null && _hitTimer >= Owner.EnemyData.StaggerTime)
        {
            // 시퀀스가 아직 실행 중이 아닐 때 (예: 시퀀스 생성 실패 또는 매우 짧은 시퀀스)
            // 또는 시퀀스가 이미 완료되었지만 OnComplete가 아직 호출되지 않은 극히 드문 경우 대비
            if (_airSequence == null || !_airSequence.IsActive())
            {
                if (!Owner.IsInAir) // 공중 상태가 아니라면 Idle로 (공중 상태는 DownedState로 별도 처리)
                {
                    SuperMachine.ChangeState<IdleState>();
                }
                else if (!SuperMachine.CurrentStateIs<DownedState>()) // 아직 DownedState로 가지 않았다면
                {
                    // 이 경우는 PlayAirborneKnockbackSequence의 OnComplete가 실패했거나 다른 문제가 있을 수 있음
                    // 안전하게 DownedState로 보내거나 Idle로 보낼 수 있음
                    SuperMachine.ChangeState<DownedState>();
                }
                return;
            }
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        _airSequence?.Kill(); // 상태 종료 시 DOTween 시퀀스 확실히 정리
        _hitTimer = 0f;       // 타이머 리셋

        // NavMeshAgent 재활성화 및 위치 동기화
        if (Owner.NavMeshAgent != null && !Owner.NavMeshAgent.enabled)
        {
            Owner.NavMeshAgent.enabled = true;
            // NavMeshAgent.Warp는 NavMesh 위에 있을 때만 호출하는 것이 안전합니다.
            // NavMeshAgent.SamplePosition 등을 사용하여 가장 가까운 NavMesh 위치를 찾은 후 Warp 할 수도 있습니다.
            if(Owner.NavMeshAgent.isOnNavMesh)
                 Owner.NavMeshAgent.Warp(Owner.transform.position);
        }

        // Owner.IsInAir = false; // 공중 상태는 착지 시 또는 DownedState 진입 시 관리되는 것이 적절
        // Owner.SetRigidbodyKinematic(false); // Rigidbody가 있다면 Kinematic 해제
    }

    // 캐릭터의 발 아래 지면 Y 좌표를 찾는 메서드 (예시)
    // private float FindGroundY(Vector3 characterPosition)
    // {
    //     RaycastHit hit;
    //     // 캐릭터 위치 (또는 발 위치)에서 아래로 레이캐스트
    //     if (Physics.Raycast(characterPosition + Vector3.up * 0.1f, Vector3.down, out hit, 5f, Owner.GroundLayerMask)) // GroundLayerMask는 Owner에 정의
    //     {
    //         return hit.point.y + Owner.GroundedOffsetY; // GroundedOffsetY는 Owner에 정의 (발 높이 보정)
    //     }
    //     return characterPosition.y; // 지면을 못 찾으면 현재 Y 유지 또는 기본값
    // }
}