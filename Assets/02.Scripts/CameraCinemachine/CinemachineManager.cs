using UnityEngine;
using UnityEngine.Playables;

public class CinemachineManager : MonoBehaviour
{
    public PlayableDirector Director;

    public void BossAppear()
    {
        Director.gameObject.SetActive(true);
        Director.Play();
    }
    
}
