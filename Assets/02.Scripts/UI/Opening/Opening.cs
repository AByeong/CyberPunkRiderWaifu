using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Opening : MonoBehaviour
{
    public Image TargetImage; // Inspector에서 할당하거나 코드로 연결
    public float FadeDuration = 2f; // n초 (페이드 인 지속 시간)
    public GameObject StartMenuUI;

    public void OffImage()
    {
        TargetImage.enabled = false;
        StartMenuUI.gameObject.SetActive(false);
    }

    public void FadeInImage()
    {
        TargetImage.enabled = true;
        if (TargetImage == null)
        {
            Debug.LogWarning("Image is null.");
            return;
        }

        // 처음에는 완전 투명
        Color color = TargetImage.color;
        color.a = 0f;
        TargetImage.color = color;

        // DOFade를 통해 alpha를 1로 n초 동안 서서히 변경
        TargetImage.DOFade(1f, FadeDuration).OnComplete(() => { StartMenuUI.SetActive(true); });
    }

    public void GameStart()
    {
        SceneManager.LoadScene("KBJ_Lobby");
    }

    public void GameExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}