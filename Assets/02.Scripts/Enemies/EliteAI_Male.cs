using System.Collections;
using UnityEngine;

public class EliteAI_Male : EliteEnemy
{
    public GameObject SummonObject;
    public Transform SummonTransform;
    public float SummonTime;
    public GameObject SummonEffect;


    private void Start()
    {
        StartCoroutine(Summoning());
    }


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
        KickVFX.SetActive(true);
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
