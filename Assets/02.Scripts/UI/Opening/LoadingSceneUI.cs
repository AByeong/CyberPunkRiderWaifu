using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneUI : MonoBehaviour
{
    public int NextSceneIndex = 2;

    public Slider ProgressSlider;

    public TextMeshProUGUI ProgressText;

    private void Start()
    {
        StartCoroutine(LoadNextScene_Coroutine());
    }

    private IEnumerator LoadNextScene_Coroutine()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(NextSceneIndex);
        ao.allowSceneActivation = false;

        while (ao.isDone == false)
        {
            Debug.Log(ao.progress); // 0 ~ 1값
            ProgressSlider.value = ao.progress;
            ProgressText.text = $"{ao.progress * 100f}%";

            if (ao.progress >= 0.9f)
            {
                ao.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}