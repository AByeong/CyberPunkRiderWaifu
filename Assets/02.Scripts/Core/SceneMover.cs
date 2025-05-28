using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
// using UnityEngine.UI; // 로딩 씬의 프로그레스 바 UI를 여기서 직접 제어한다면 필요

public class SceneMover : Singleton<SceneMover>
{
    public string LoadingSceneName;
    private AsyncOperation operation;

    

    public void ShowDungeonPopup(string dungeonName)
    {
        UIManager.Instance.PopupManager.ShowAnswerPopup("배달하러 가시겠습니까?", "당근 빳따죠!", "아, 잠만요", () =>
        {
            UIManager.Instance.PopupManager.CloseAllPopups();
            UIManager.Instance.isInDelivery = true;
            UIManager.Instance.isCursorLockNeed = true;

            // 1. 동기적으로 로딩 씬으로 이동
            MovetoScene(LoadingSceneName);

            // 2. 로딩 씬으로 이동 후, 실제 목적지 씬을 비동기로 로드 시작
            //    이때, SceneMover는 Singleton이므로 LoadingScene에서도 계속 살아있어야 함
            AsyncMoveScene(dungeonName);

        }, numberofbuttons: 2);
    }

    public void AsyncMoveScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsynchronously(sceneName));
    }

    private void Update()
    {
       
            if (Input.GetKeyDown(KeyCode.Escape)) // Escape를 누르면 수동으로 활성화
            {
                Debug.Log("Escape 키 입력으로 씬 활성화 시도.");
                LoadAsyncComplete();
            }
        
    }

    IEnumerator LoadSceneAsynchronously(string sceneName)
    {
        // 비동기적으로 다음 씬 로드 시작
        operation = SceneManager.LoadSceneAsync(sceneName);

        // 처음에는 자동 씬 전환을 막습니다. (로딩 진행률을 끝까지 보기 위함)
        operation.allowSceneActivation = false;

        Debug.Log(sceneName + " 씬 비동기 로드 시작...");

        // 로딩이 90% 완료될 때까지 반복 (실제로는 operation.progress가 0.9가 될 때까지)
        while (operation.progress < 0.9f)
        {
            // operation.progress 값은 0.0에서 0.9까지만 진행됨
            float displayProgress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log(sceneName + " 로딩 진행률: " + (displayProgress * 100f) + "%");

            // TODO: 여기에 LoadingSceneName에 있는 UI 프로그레스 바를 업데이트하는 로직 추가
            // 예: if (loadingProgressBar != null) loadingProgressBar.value = displayProgress;
            //     (SceneMover가 직접 UI를 알거나, LoadingScene의 UI 스크립트가 SceneMover.Instance.GetProgress() 같은 함수를 호출)

            yield return null; // 다음 프레임까지 대기
        }

        // 로딩이 90% 완료됨.
        Debug.Log(sceneName + " 씬 로드 거의 완료 (90%). 자동 활성화를 준비합니다.");
        // 여기서 "로딩 완료! 아무 키나 누르세요..." 같은 메시지를 잠깐 보여줄 수도 있습니다.
        yield return new WaitForSeconds(1f); // 예시: 1초 대기

        // "로딩이 다 되면 바로 다음 씬으로 이동되도록" 하기 위해 allowSceneActivation을 true로 설정
        operation.allowSceneActivation = false;
        // 씬이 완전히 활성화될 때까지 대기 (isDone이 true가 될 때까지)
        while (!operation.isDone)
        {
            yield return null;
        }

        Debug.Log(sceneName + " 씬 로드 및 활성화 완료!");
        operation = null; // 작업 완료 후 operation 참조 해제
    }

    // 이 함수는 Update의 Escape 키 또는 다른 외부 호출에 의해 수동으로 씬을 활성화할 때 사용
    public void LoadAsyncComplete()
    {
        if (operation != null)
        {
            operation.allowSceneActivation = true;
        }
    }

    


    public void MovetoScene(string sceneName)
    {
        if (UIManager.Instance != null && UIManager.Instance.PopupManager != null && UIManager.Instance.PopupManager.PopupStack != null)
        {
            UIManager.Instance.PopupManager.PopupStack.Clear();
        }
        SceneManager.LoadScene(sceneName);
    }
}