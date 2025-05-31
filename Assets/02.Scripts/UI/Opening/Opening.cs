using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Opening : MonoBehaviour
{
    public Image targetImage; // Inspector에서 할당하거나 코드로 연결
    public float fadeDuration = 2f; // n초 (페이드 인 지속 시간)
    public GameObject StartMenuUI;

    public void OffImage()
    {
        targetImage.enabled = false;
        StartMenuUI.gameObject.SetActive(false);
    }

    public void FadeInImage()
    {
        targetImage.enabled = true;
        if (targetImage == null)
        {
            Debug.LogWarning("Image is null.");
            return;
        }

        // 처음에는 완전 투명
        Color color = targetImage.color;
        color.a = 0f;
        targetImage.color = color;

        // DOFade를 통해 alpha를 1로 n초 동안 서서히 변경
        targetImage.DOFade(1f, fadeDuration).OnComplete(() => { StartMenuUI.SetActive(true); });
    }

    public void GameStart()
    {
        SceneManager.LoadScene(1);
    }
}