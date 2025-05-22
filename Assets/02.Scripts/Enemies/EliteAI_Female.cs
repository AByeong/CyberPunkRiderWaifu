using UnityEngine;

public class EliteAI_Female : MonoBehaviour
{
    public Animator EliteAnimator;
    public void Running()
    {
        EliteAnimator.SetBool("Running", true);
    }

    public void NotRunning()
    {
        EliteAnimator.SetBool("Running", false);
    }
}
