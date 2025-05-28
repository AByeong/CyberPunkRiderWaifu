using System.Collections.Generic;
// NUnit.Framework는 일반적으로 유닛 테스트용이므로 MonoBehaviour 스크립트에서는 제거해도 괜찮습니다.
// using NUnit.Framework; 
using UnityEngine;
using UnityEngine.VFX;

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

    
    [SerializeField]
    private ElliteStateMachine _elliteStateMachineite;
    
    
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
        
    }

    public void StampStep()
    {
        Vector3 sphereCenter = StampPosition.position;
        Collider[] detectedColliders = Physics.OverlapSphere(sphereCenter, StampRange);
        int affectedCount = 0; // 영향을 받은 개체 수 카운트

//        Debug.Log($"[StampStep] Detected {detectedColliders.Length} colliders in range.");

        foreach (Collider hitCollider in detectedColliders)
        {
            if (hitCollider.tag == "Elite") continue;

            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
//                Debug.Log($"[StampStep] Applying NoDamageButAir to: {hitCollider.gameObject.name}");
                Damage damage = new Damage();
                damage.DamageValue = 0;
                damage.DamageType = EDamageType.Airborne;
                damageable.TakeDamage(damage);
                affectedCount++;
            }
        }
//        Debug.Log($"[StampStep] Total {affectedCount} IDamageable objects affected.");
    }

public TrailRenderer EyeTrail;

public void EyeTurnOn()
{
    EyeTrail.gameObject.SetActive(true);
}

public void EyeTurnOff()
{
    EyeTrail.gameObject.SetActive(false);
}


public GameObject KingStompVFX;
public void KingStompAttack()
{
    
    KingStompVFX.SetActive(true);
    KingStompVFX.GetComponent<ParticleSystem>().Play();
}


    public GameObject StompVFX;
    public void StompAttack()
    {
        _elliteStateMachineite.ChangeState<EliteAttackState>();
        StompVFX.SetActive(true);
        StompVFX.GetComponent<ParticleSystem>().Play();
    }

    public GameObject TornadoVFX;

    public void TornadoAttack()
    {
        TornadoVFX.SetActive(true);
        TornadoVFX.GetComponent<ParticleSystem>().Play();
    }


    public void TornadoAttackEnd()
    {
        TornadoVFX.SetActive(false);
        TornadoVFX.GetComponent<VisualEffect>().Stop();
        TornadoVFX.GetComponent<ParticleSystem>().Stop();
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


}

