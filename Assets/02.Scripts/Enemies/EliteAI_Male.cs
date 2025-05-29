using System.Collections;
using UnityEngine;

public class EliteAI_Male : EliteEnemy
{
    public GameObject SummonObject;
    public Transform SummonTransform;
    public float SummonTime;


    private void Start()
    {
        StartCoroutine(Summoning());
    }


    // 패턴 1
    public GameObject StompVFX;
    public void Stomp()
    {
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
    
    public IEnumerator Summoning()
    {
        while (true)
        {
            yield return new WaitForSeconds(SummonTime);
        }
        
    }
}
