using UnityEngine;

public class EliteAI_Female : EliteEnemy
{
    public TrailRenderer EyeTrail;
    public float StampRange;
    public Transform StampPosition; // 현재 StampStep에서는 사용되지 않지만, 기즈모에서 활용 가능
    [SerializeField] private float runCooldownDuration = 3.0f;

    private bool isCoolingDown = false;
    private float currentCooldownTime = 0f;


    private Damage _tornadoDamage;
    private Damage _stompDamage;
    private Damage _kingStopmDamage;

    protected override void Awake()
    {
        base.Awake();

        _tornadoDamage = new Damage()
        {
            DamageValue = 2,
            DamageType = EDamageType.Normal,
            DamageForce = 0.5f,
            From = gameObject
        };

        _stompDamage = new Damage()
        {
            DamageValue = 10,
            DamageType = EDamageType.Normal,
            DamageForce = 0.5f,
            From = gameObject
        };

        _kingStopmDamage = new Damage()
        {
            DamageValue = 20,
            DamageType = EDamageType.Airborne,
            DamageForce = 2f,
            AirRiseAmount = 2f,
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
        if (_navMeshAgent.velocity.magnitude < 5f)
        {
            return;
        }

        Attack(StampPosition.position, StampRange, _enemyDamage, true);

        LookAtTarget();

        SoundManager.Instance.Play(SoundType.Elite_Female_Step);
    }


    // 특수 패턴
    public GameObject KingStompVFX;
    public void KingStompAttack()
    {
        Attack(StampPosition.position, StampRange, _kingStopmDamage, true);

        SoundManager.Instance.Play(SoundType.Elite_Female_KingStamp);

        KingStompVFX.SetActive(true);
        KingStompVFX.GetComponent<ParticleSystem>().Play();
    }

    // 패턴1
    public GameObject StompVFX;
    public void StompAttack()
    {
        LookAtTarget();
        Attack(StampPosition.position, StampRange, _stompDamage);

        SoundManager.Instance.Play(SoundType.Elite_Electricity);

        StompVFX.SetActive(true);
        StompVFX.GetComponent<ParticleSystem>().Play();
    }



    //  패턴2
    public GameObject TornadoVFX;
    public void TornadoAttack()
    {
        LookAtTarget();

        SoundManager.Instance.Play(SoundType.Elite_Female_Tornado);

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

