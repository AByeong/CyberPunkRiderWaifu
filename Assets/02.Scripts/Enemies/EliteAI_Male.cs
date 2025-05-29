using System.Collections;
using UnityEngine;

public class EliteAI_Male : EliteEnemy
{
    public GameObject SummonObject;
    public Transform SummonTransform;
    public float SummonTime;
    public GameObject SummonEffect;
    public Transform KickTransfrom;

    [SerializeField] private float _kickRange = 3f;
   


    // 패턴 1
    public GameObject StompVFX;
    public void Stomp()
    {
        SoundManager.Instance.Play(SoundType.Elite_Electricity);

        StompVFX.SetActive(true);
        StompVFX.GetComponent<ParticleSystem>().Play();
    }

    // 패턴 2
    public GameObject KickVFX;
    public void Kick()
    {
        SoundManager.Instance.Play(SoundType.Elite_male_Kick);
        
        Vector3 sphereCenter = KickTransfrom.position;
        Collider[] detectedColliders = Physics.OverlapSphere(sphereCenter, _kickRange);
        
        
        foreach (Collider hitCollider in detectedColliders)
        {
            if (hitCollider.tag == "Elite") continue;

            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Debug.Log($"[StampStep] Applying NoDamageButAir to: {hitCollider.gameObject.name}");
                Damage damage = new Damage();
                damage.DamageValue = 0;
                damage.DamageType = EDamageType.Normal;
                damage.DamageForce = 10f;
                damage.From = this.gameObject;
                damageable.TakeDamage(damage);
            }
        }
        
        
        
        
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
        SoundManager.Instance.Play(SoundType.Elite_male_Summon);
        Instantiate(SummonEffect, SummonTransform.position, SummonTransform.rotation);    
    }
}
