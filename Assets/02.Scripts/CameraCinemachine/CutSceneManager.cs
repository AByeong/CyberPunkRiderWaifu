using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    private CinemachineManager _cinemachineManager;
    private void Start()
    {
        _cinemachineManager = FindFirstObjectByType<CinemachineManager>();
    }
    public void EndCutScene(string sceneName)
    {
        _cinemachineManager.EndCutScene(sceneName);
    }

    public void EndStage()
    {
        Debug.Log($"{gameObject.name} :: SceneChanging");
        StartCoroutine(LoadLobby());
    }

    private IEnumerator LoadLobby()
    {
        Time.timeScale = 1f;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("KBJ_Lobby", LoadSceneMode.Single);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        
    }
}
