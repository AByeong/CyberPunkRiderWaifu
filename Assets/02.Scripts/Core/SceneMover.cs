using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover : MonoBehaviour
{
    public void MovetoScene(string sceneName)
    {
        UIManager.Instance.PopupManager.PopupStack.Clear();
        SceneManager.LoadScene(sceneName);
    }
}
