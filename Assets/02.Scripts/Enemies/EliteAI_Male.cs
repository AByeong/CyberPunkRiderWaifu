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
            EliteAnimator.SetTrigger("Summoning");
            
        }
        
    }

    public void Summon()
    {
        Instantiate(SummonObject, SummonTransform.position, SummonTransform.rotation);
    }   
}
