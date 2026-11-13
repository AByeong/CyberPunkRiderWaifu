using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMover : Singleton<SceneMover>
{
    public string LoadingSceneName = "KBJ_DungeonIntro"; // 로딩씬 이름
    public string TargetSceneName;                      // 실제 목적지 씬 이름
    private AsyncOperation asyncOperation;
    private bool isLoadingEnd = false;

    protected override void Awake()
    {
        base.Awake();

    }

    private void Start()
    {
        SoundManager.Instance.PlayBGM(SoundType.BGM_OfficeStage);
        GameManager.Instance.OnReturnToLobby += Initialize;
    }

    public void Initialize()
    {
        Button startBtn;
        if (GameObject.FindWithTag("StartButton").TryGetComponent<Button>(out startBtn))
        {
            startBtn.onClick.AddListener(() => ShowDungeonPopup("KBJ_Procedure"));
        }
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
        SceneManager.LoadScene(sceneName); // 참조 재바인딩은 UIManager가 sceneLoaded에서 처리

    }


    public void MoveToLobby()
    {
        UIManager.Instance?.PopupManager?.PopupStack?.Clear();
        StartCoroutine(LoadLobby());
    }

    public bool IsLoadingLobby { get; private set; }

    private IEnumerator LoadLobby()
    {
        if (IsLoadingLobby) yield break; // 이미 로비로 이동 중
        IsLoadingLobby = true;

        GameManager.Instance.GameReplay();
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("KBJ_Lobby", LoadSceneMode.Single);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        IsLoadingLobby = false;
    }
}
