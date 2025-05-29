using System.Collections;
using UnityEngine;

public class EliteAI_Male : MonoBehaviour
{
    public Animator EliteAnimator;
    public GameObject SummonObject;
    public Transform SummonTransform;

    public float SummonTime;
    public GameObject SummonEffect;
    public GameObject StompVFX;

    public void Stomp()
    {
        SoundManager.Instance.Play(SoundType.Elite_Electricity);

        StompVFX.SetActive(true);
        StompVFX.GetComponent<ParticleSystem>().Play();
    }


    public GameObject KickVFX;

    public void Kick()
    {
        KickVFX.SetActive(true);
        KickVFX.GetComponent<ParticleSystem>().Play();
    }
    

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
