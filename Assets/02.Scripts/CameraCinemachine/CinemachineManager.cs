using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class CinemachineManager : Singleton<CinemachineManager>
{
    public PlayableDirector Director;
    public CinemachineCamera PlayerCamera;
    public void BossAppear()
    {
        Director.Play();
        PlayerCamera.gameObject.SetActive(false);
    }

    public void BossAppearEndEvent()
    {
        PlayerCamera.gameObject.SetActive(true);
    }
    
}
