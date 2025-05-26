using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StageManager.Instance.ExitPortal = gameObject;
            StageManager.Instance.MoveNextStage();
        }
    }
}
