using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover : Singleton<SceneMover>
{

    public string LoadingSceneName;
    public void ShowDungeonPopup(string dungeonName)
    {
        
        
        UIManager.Instance.PopupManager.ShowAnswerPopup("배달하러 가시겠습니까?","당근 빳따죠!","아, 잠만요",() =>
        {
            UIManager.Instance.PopupManager.CloseAllPopups();
            
            //이제 던전에 들어가고 커서 락이 필요함
            UIManager.Instance.isInDelivery = true;
            UIManager.Instance.isCursorLockNeed = true;
            MovetoScene(LoadingSceneName);
            AsyncMoveScene(dungeonName);
           
        },numberofbuttons : 2);

    }

    public void AsyncMoveScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsynchronously(sceneName));
    }
    
    IEnumerator LoadSceneAsynchronously(string sceneName)
    {
        // 비동기적으로 다음 씬 로드 시작
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        operation.allowSceneActivation = false; // 로드가 완료되어도 바로 넘어가지 않게 하려면 주석 해제
        // 이 경우, 특정 조건 만족 시 operation.allowSceneActivation = true; 로 설정하여 씬 전환

        // 로딩이 완료될 때까지 반복
        while (!operation.isDone)
        {
           
            // 실제 로딩 진행률을 좀 더 정확하게 표시하려면 추가 계산이 필요할 수 있음
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            

            Debug.Log("로딩 진행률: " + (progress * 100f) + "%");
            

            yield return null; // 다음 프레임까지 대기
        }

        // 로딩이 완료되면 이 부분이 실행됨 
        Debug.Log("다음 씬 로드 완료!");
        
    }

    public void LoadAsyncComplete()
    {
        operation.allowSceneActivation = true;
    }
    
    public void MovetoScene(string sceneName)
    {
        
        UIManager.Instance.PopupManager.PopupStack.Clear();
        SceneManager.LoadScene(sceneName);

    }
}
