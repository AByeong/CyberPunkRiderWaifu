using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover : Singleton<SceneMover>
{
    public void MovetoScene(string sceneName)
    {
        UIManager.Instance.PopupManager.PopupStack.Clear();
        SceneManager.LoadScene(sceneName);
    }
}
