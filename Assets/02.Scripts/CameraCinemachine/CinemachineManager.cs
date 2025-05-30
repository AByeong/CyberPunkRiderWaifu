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
   
    public CinemaName CinemaName;
   

    private void AnimationStart(CinemaName cinemaName)
    {
        //시간을 멈춘다, 플레이어 카메라를 끈다.
        Time.timeScale = 0;
        PlayerCamera.gameObject.SetActive(false);
        Debug.Log($"{cinemaName}을 재생합니다.");
        Director.Play(TimelinePreferences[(int)cinemaName]);
    }
    
    public void AnimationEnd()
    {
        
        //멈춰있던 시간을 돌리고, 플레이어 카메라를 키고 레이어를 everything으로 한다.
        Time.timeScale = 1;
        PlayerCamera.gameObject.SetActive(true);
        Camera.main.cullingMask = ~0;
        
    }
    
    
    
    
    
    public void ShowBossAppear()
    {
        Debug.Log("보스 시네머신 시작");

        
        AnimationStart(CinemaName.BossAppear);
    }
    
    public void EndBossAppear()
    {
        EnemyManager.Instance.SpawnBoss();
        AnimationEnd();
    }
    

    

    public void ShowElevatorChangeAnimation()
    {
        Debug.Log("엘리베이터 시네머신 시작");
        Camera.main.cullingMask = Camera.main.cullingMask = 1 << LayerMask.NameToLayer("Cinemachine");

        AnimationStart(CinemaName.ElevatorChange);
    }

    public void EndElevatorChangeAnimation()
    {
       
        PlayerCamera.gameObject.SetActive(true);
        Camera.main.cullingMask = ~0;
        
        AnimationEnd();
        Debug.Log("엘리베이터 시네머신 종료");
        if (DeliveryManager.Instance.CurrentSector == DeliveryManager.Instance.CompleteSector-1)
        {
            ShowBossAppear();
        }
        
    }

    public void EndSkill()
    {
       // PlayerCamera.gameObject.SetActive(true);
    }
}
