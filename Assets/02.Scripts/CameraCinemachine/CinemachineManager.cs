using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
public enum CinemaName
{
    BossAppear,
    ElevatorChange
}

public class CinemachineManager : Singleton<CinemachineManager>
{
    public PlayableDirector Director;
    public List<TimelineAsset> TimelinePreferences;
    public CinemachineCamera PlayerCamera;
public GameObject Player;
   
public CinemaName CinemaName;
   

    private void AnimationStart(CinemaName cinemaName)
    {
        Debug.Log($"{cinemaName}을 재생합니다.");
        Director.Play(TimelinePreferences[(int)cinemaName]);
        //PlayerCamera.gameObject.SetActive(false);
    }
    
    public void BossAppear()
    {
        AnimationStart(CinemaName.BossAppear);
    }

    public void AnimationEnd()
    {
        Player.GetComponent<Animator>().applyRootMotion = true;
        Player.GetComponent<CharacterController>().enabled = true;
        PlayerCamera.gameObject.SetActive(true);
        Camera.main.cullingMask = ~0;
        
    }

    public void ShowElevatorChangeAnimation()
    {
        Player.GetComponent<Animator>().applyRootMotion = false;
        Player.GetComponent<CharacterController>().enabled = false;

        Camera.main.cullingMask = Camera.main.cullingMask = 1 << LayerMask.NameToLayer("Player");

        AnimationStart(CinemaName.ElevatorChange);
    }

    public void ElevatorChangeEnd()
    {
        DeliveryManager.Instance.StageManager.MoveNextStage();
        Player.GetComponent<Animator>().applyRootMotion = true;
        Player.GetComponent<CharacterController>().enabled = true;
        PlayerCamera.gameObject.SetActive(true);
        Camera.main.cullingMask = ~0;
    }

    public void EndSkill()
    {
       // PlayerCamera.gameObject.SetActive(true);
    }
}
