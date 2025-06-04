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
        SceneMover.Instance.MoveToLobby();
    }
}
