using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StageManager.Instance.ExitPortal = gameObject;
            StageManager.Instance.MoveNextStage();
        }
    }
}
