using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DeliveryManager.Instance.StageManager.ExitPortal = gameObject;
            DeliveryManager.Instance.StageManager.MoveNextStage();
        }
    }
}
