using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover : Singleton<SceneMover>
{
    public string LoadingSceneName = "KBJ_DungeonIntro"; // 로딩씬 이름
    public string TargetSceneName;                      // 실제 목적지 씬 이름
    private AsyncOperation asyncOperation;
    private bool isLoadingEnd = false;

    private void Awake()
    {
        base.Awake();
//        DontDestroyOnLoad(this.gameObject); // 씬이 바뀌어도 파괴되지 않음
    }

    private void Start()
    {
        SoundManager.Instance.PlayBGM(SoundType.BGM_OfficeStage);
    }

    public void ShowDungeonPopup(string dungeonName)
    {
        UIManager.Instance.PopupManager.ShowAnswerPopup("배달하러 가시겠습니까?", "당근 빳따죠!", "아, 잠만요", () =>
        {
            UIManager.Instance.PopupManager.CloseAllPopups();
            UIManager.Instance.isInDelivery = true;
            UIManager.Instance.isCursorLockNeed = true;

            TargetSceneName = dungeonName; // 최종 목적지 씬 이름 저장
            MovetoScene(LoadingSceneName); // 1단계: 로딩 씬으로 먼저 이동
        }, numberofbuttons: 2);
    }

   

   

    public void LoadAsyncComplete()
    {
        isLoadingEnd = true;
        Debug.Log("LoadAsyncComplete() 호출됨 - 씬 활성화 준비 완료");
    }

    public void MovetoScene(string sceneName)
    {
        UIManager.Instance?.PopupManager?.PopupStack?.Clear();
        SceneManager.LoadScene(sceneName);
    }
}
