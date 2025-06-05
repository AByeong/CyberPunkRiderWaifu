using System;
using System.Collections;
using System.Collections.Generic;
using JY;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
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

    public PlayerInput PlayerInput;

    private void Start()
    {
        PlayerInput = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();
    }

    private void AnimationStart(CinemaName cinemaName)
    {
        //시간을 멈춘다, 플레이어 카메라를 끈다.
        GameManager.Instance.GameStop();
        PlayerInput.ReleaseControl();
        PlayerCamera.gameObject.SetActive(false);
        Debug.Log($"{cinemaName}을 재생합니다.");
        Director.Play(TimelinePreferences[(int)cinemaName]);


        Camera.main.cullingMask = 1 << LayerMask.NameToLayer("Cinemachine");
    }
    
    public void AnimationEnd()
    {
        GameManager.Instance.GameReplay();
        PlayerInput.GainControl();
        PlayerCamera.gameObject.SetActive(true);
        Camera.main.cullingMask = ~LayerMask.GetMask("MiniMap");
        UIManager.Instance.ActivateMainUI();
    }

    public void ShowBossAppear()
    {
        Debug.Log("보스 시네머신 시작");
        
        StartCoroutine(LoadCutScene("KBJ_Boss1Appear"));
        EnemyManager.Instance.SpawnBoss();
    }

    public void ShowBossPhase2Appear()
    {
        Debug.Log("보스 2페이즈 시네머신 시작");
        StartCoroutine(LoadCutScene("KBJ_Boss2Appear"));
    }

    public void ShowEndiding()
    {
        StartCoroutine(LoadCutScene("KBJ_DueongunClear"));
    }

    // public void EndBossAppear()
    // {
    //     EnemyManager.Instance.SpawnBoss();
    //     AnimationEnd();
    // }

    public void EndCutScene(string sceneName)
    {
        StartCoroutine(UnloadCutScene(sceneName));
    }

    private IEnumerator LoadCutScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
            
        }
        

        Time.timeScale = 0;
        PlayerCamera.gameObject.SetActive(false);
        UIManager.Instance.DeactivateMainUI();
    }

    private IEnumerator UnloadCutScene(string sceneName)
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        AnimationEnd();
    }
    

    public void ShowElevatorChangeAnimation()
    {
        Debug.Log("엘리베이터 시네머신 시작");

        AnimationStart(CinemaName.ElevatorChange);
    }

    public void EndElevatorChangeAnimation()
    {
        if (DeliveryManager.Instance.CurrentSector == DeliveryManager.Instance.CompleteSector - 1)
        {
            ShowBossAppear();
        }
        else
        {
            AnimationEnd();
        }
        
    }

    public void EndSkill()
    {
       // PlayerCamera.gameObject.SetActive(true);
    }
}
