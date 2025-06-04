using UnityEngine;

public class EliteAI_Male : EliteEnemy
{
    [Header("[Summon Parameter]")]
    public GameObject SummonObject;
    public Transform SummonTransform;
    public float SummonTime;
    public GameObject SummonEffect;

    [Header("[Kick/Stopm Parameter]")]
    public Transform KickTransfrom;
    [SerializeField] private float _kickRange = 4f;
    [SerializeField] private float _stompRange = 3f;

    private Damage _kickDamage;
    private Damage _stompDamage;

    protected override void Awake()
    {
        base.Awake();

        _kickDamage = new Damage()
        {
            DamageValue = 10,
            DamageType = EDamageType.Airborne,
            DamageForce = 10f,
            AirRiseAmount = 2f,
            From = gameObject
        };

        _stompDamage = new Damage()
        {
            DamageValue = 10,
            DamageType = EDamageType.Normal,
            DamageForce = 0.5f,
            From = gameObject
        };
    }
   

    private void LookAtTarget()
    {

        Vector3 direction = (GameManager.Instance.player.transform.position - transform.position).normalized;
        direction.y = 0f; // 수평만 회전하도록
        transform.forward = direction;
    }
    

    // 패턴 1
    public GameObject StompVFX;
    public void Stomp()
    {
        LookAtTarget();

        Attack(KickTransfrom.position, _stompRange, _stompDamage);

        SoundManager.Instance.Play(SoundType.Elite_Electricity);

        StompVFX.SetActive(true);
        StompVFX.GetComponent<ParticleSystem>().Play();
    }

    // 패턴 2
    public GameObject KickVFX;
    public void Kick()
    {
        LookAtTarget();

        Attack(KickTransfrom.position, _kickRange, _kickDamage, true);

        SoundManager.Instance.Play(SoundType.Elite_male_Kick);

        KickVFX.SetActive(true);
        KickVFX.transform.position = KickTransfrom.position;
        KickVFX.GetComponent<ParticleSystem>().Play();
    }

    // 패턴 3
    public void Summon()
    {
        Instantiate(SummonObject, SummonTransform.position, SummonTransform.rotation);
    }

    public void SummonStart()
    {
        LookAtTarget();

        SoundManager.Instance.Play(SoundType.Elite_male_Summon);
        Instantiate(SummonEffect, SummonTransform.position, SummonTransform.rotation);    
    }
}
