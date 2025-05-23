using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class CinemachineManager : Singleton<CinemachineManager>
{
    public List<PlayableDirector> Director;
    public CinemachineCamera PlayerCamera;
    public void BossAppear()
    {
        Director[0].Play();
        PlayerCamera.gameObject.SetActive(false);
    }

    public void BossAppearEndEvent()
    {
        PlayerCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Director[1].Play();
            PlayerCamera.gameObject.SetActive(false);
        }
    }

    public void EndSkill()
    {
        PlayerCamera.gameObject.SetActive(true);
    }
}
