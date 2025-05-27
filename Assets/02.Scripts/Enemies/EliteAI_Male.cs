using System.Collections;
using UnityEngine;

public class EliteAI_Male : MonoBehaviour
{
    public Animator EliteAnimator;
    public GameObject SummonObject;
    public Transform SummonTransform;

    public float SummonTime;


    private void Start()
    {
        StartCoroutine(Summoning());
    }
    
    public IEnumerator Summoning()
    {
        while (true)
        {
            yield return new WaitForSeconds(SummonTime);

            
        }
        
    }

    public GameObject StompVFX;

    public void Stomp()
    {
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
}
