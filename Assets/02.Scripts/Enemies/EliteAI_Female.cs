using System.Collections.Generic;
// NUnit.Framework는 일반적으로 유닛 테스트용이므로 MonoBehaviour 스크립트에서는 제거해도 괜찮습니다.
// using NUnit.Framework; 
using UnityEngine;

public class EliteAI_Female : MonoBehaviour
{
    public Animator EliteAnimator;

    public float StampRange;
    public Transform StampPosition; // 현재 StampStep에서는 사용되지 않지만, 기즈모에서 활용 가능

    [SerializeField]
    private float runCooldownDuration = 3.0f;

    private bool isCoolingDown = false;
    private float currentCooldownTime = 0f;
    private Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    void Update()
    {
        if (isCoolingDown)
        {
            currentCooldownTime -= Time.deltaTime;
            if (currentCooldownTime <= 0)
            {
                isCoolingDown = false;
                currentCooldownTime = 0f;
            }
        }
        
        DrawGizmosSelected();
    }

    public void StampStep()
    {
        Vector3 sphereCenter = StampPosition.position;
        Collider[] detectedColliders = Physics.OverlapSphere(sphereCenter, StampRange);
        int affectedCount = 0; // 영향을 받은 개체 수 카운트

        Debug.Log($"[StampStep] Detected {detectedColliders.Length} colliders in range.");

        foreach (Collider hitCollider in detectedColliders)
        {
            //if (hitCollider.gameObject == gameObject) continue;

            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Debug.Log($"[StampStep] Applying NoDamageButAir to: {hitCollider.gameObject.name}");
                Damage damage = new Damage();
                damage.DamageValue = 0;
                damage.DamageType = EDamageType.NoDamageButAir;
                damageable.TakeDamage(damage);
                affectedCount++;
            }
        }
        Debug.Log($"[StampStep] Total {affectedCount} IDamageable objects affected.");
    }

    public void StepAttack()
    {
        // 이 함수는 아직 구현되지 않았습니다.
    }

    public void Running()
    {
        if (!isCoolingDown)
        {
            EliteAnimator.SetBool("Running", true);
            if(_collider != null) _collider.enabled = false; // 콜라이더가 있는 경우에만 비활성화
        }
    }

    public void NotRunning()
    {
        EliteAnimator.SetBool("Running", false);
        if(_collider != null) _collider.enabled = true; // 콜라이더가 있는 경우에만 활성화
        isCoolingDown = true;
        currentCooldownTime = runCooldownDuration;
    }

#if UNITY_EDITOR // 유니티 에디터에서만 실행되도록 전처리기 지시문 사용
    private void DrawGizmosSelected()
    {
        // 기즈모 색상 설정 (예: 노란색, 반투명)
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.3f); // Yellow with alpha

        // StampStep에서 사용하는 OverlapSphere의 범위를 그립니다.
        // 현재 StampStep은 transform.position을 중심으로 사용합니다.
        // 만약 StampPosition Transform을 중심으로 하고 싶다면 아래의 첫 번째 인자를 StampPosition.position으로 변경하세요.
        Vector3 gizmoCenter = transform.position;

        // StampPosition 변수가 할당되어 있고, 이를 사용하고 싶다면 아래 주석을 해제하고 위 gizmoCenter 정의를 주석 처리하세요.
        // if (StampPosition != null)
        // {
        //     gizmoCenter = StampPosition.position;
        // }

        Gizmos.DrawSphere(gizmoCenter, StampRange);

        // 와이어 프레임으로 그리고 싶다면 DrawWireSphere 사용
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(gizmoCenter, StampRange);
    }
#endif
}

