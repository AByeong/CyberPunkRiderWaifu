using UnityEngine;

public class EliteAI_Female : EliteEnemy
{
    public TrailRenderer EyeTrail;
    public float StampRange;
    public Transform StampPosition; // 현재 StampStep에서는 사용되지 않지만, 기즈모에서 활용 가능
    [SerializeField] private float runCooldownDuration = 3.0f;

    private bool isCoolingDown = false;
    private float currentCooldownTime = 0f;

    private Damage _enemyDamage;

    protected override void Awake()
    {
        base.Awake();

        _enemyDamage = new Damage()
        {
            DamageValue = 0,
            DamageType = EDamageType.Airborne,
            DamageForce = 2f,
            From = gameObject
        };
        
    }

    protected override void Update()
    {
        base.Update();

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

    // 이동 공격
    public void StampStep()
    {
        if (_navMeshAgent.velocity.magnitude < 4f)
        {
            return;
        }

        Vector3 sphereCenter = StampPosition.position;

        SoundManager.Instance.Play(SoundType.Elite_Female_Step);
        LookAtTarget();


        Collider[] detectedColliders = Physics.OverlapSphere(sphereCenter, StampRange);


        int numbers = 0;
        foreach (Collider hitCollider in detectedColliders)
        {
            
            if (hitCollider.tag == "NormalEnemy" || hitCollider.tag == "Player")
            {
                ++numbers;
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                {
                    damageable.TakeDamage(_enemyDamage);
                }
            }
        }
    }


    // 특수 패턴
    public GameObject KingStompVFX;
    public void KingStompAttack()
    {
        SoundManager.Instance.Play(SoundType.Elite_Female_KingStamp);
        KingStompVFX.SetActive(true);
        KingStompVFX.GetComponent<ParticleSystem>().Play();
    }

    // 패턴1
    public GameObject StompVFX;
    public void StompAttack()
    {
        SoundManager.Instance.Play(SoundType.Elite_Electricity);
        LookAtTarget();
        StompVFX.SetActive(true);
        StompVFX.GetComponent<ParticleSystem>().Play();
    }



    //  패턴2
    public GameObject TornadoVFX;
    public void TornadoAttack()
    {
        SoundManager.Instance.Play(SoundType.Elite_Female_Tornado);
        LookAtTarget();
        TornadoVFX.SetActive(true);
        TornadoVFX.GetComponent<ParticleSystem>().Play();
    }

    public void TornadoAttackEnd()
    {
        TornadoVFX.SetActive(false);
        TornadoVFX.GetComponent<ParticleSystem>().Stop();
    }


    public void Running()
    {
        if (!isCoolingDown)
        {
            _animator.SetBool("Running", true);
        }
    }

    public void NotRunning()
    {
        _animator.SetBool("Running", false);
        isCoolingDown = true;
        currentCooldownTime = runCooldownDuration;
    }


    private void LookAtTarget()
    {

        Vector3 direction = (GameManager.Instance.player.transform.position - transform.position).normalized;
        direction.y = 0f; // 수평만 회전하도록
        transform.forward = direction;
    }

    public void EyeTurnOn()
    {
        if (!EyeTrail.enabled)
        {
            SoundManager.Instance.Play(SoundType.Elite_Female_Detect);
        }
        LookAtTarget();

        EyeTrail.gameObject.SetActive(true);
    }

    public void EyeTurnOff()
    {
        EyeTrail.gameObject.SetActive(false);
    }

}

