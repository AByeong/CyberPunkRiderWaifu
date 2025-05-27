using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            DeliveryManager.Instance.StageManager.ExitPortal = gameObject;
            DeliveryManager.Instance.StageManager.CheckToMoveNextStage();
            
            
        }
    }
}
