using System;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
      //  Debug.Log($"{hit.gameObject.name}");
        IDamageable damageable = hit.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log("Player에 들어옴 ㅋ");            
        }
    }

}
