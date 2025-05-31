using UnityEngine;
[System.Serializable]
public class BlinkStep
{
    public float duration;
    public bool isOn;
}

public class CustomBlinkController : MonoBehaviour
{
    public Material mat;
    public Color baseColor = Color.cyan;
    public float intensity = 5f;
    public BlinkStep[] steps;

    private int index = 0;
    private float timer = 0f;

    void Update()
    {
        if (steps.Length == 0) return;

        timer += Time.deltaTime;
        var step = steps[index];

        if (timer >= step.duration)
        {
            timer = 0f;
            index = (index + 1) % steps.Length;

            if (steps[index].isOn)
                mat.SetColor("_EmissionColor", baseColor * Mathf.LinearToGammaSpace(intensity));
            else
                mat.SetColor("_EmissionColor", Color.black);
        }
    }
}
