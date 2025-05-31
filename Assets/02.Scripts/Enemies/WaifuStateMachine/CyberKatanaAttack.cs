using System;
using UnityEngine;

public class CyberKatanaAttack : MonoBehaviour
{
   public GameObject Boss;
   public Damage damage;

   private void OnTriggerEnter(Collider other)
   {
      Debug.Log("OnTriggerEnter");
      
      if (other.gameObject.CompareTag("Player"))
      {
         Debug.Log("Detected Player");
         if (damage.Equals(default(Damage)))
         {
            damage = new  Damage();
            damage.DamageValue = 100;
            damage.From = Boss;
            damage.DamageType = EDamageType.Normal;
         }
         
         other.gameObject.GetComponent<PlayerHit>().TakeDamage(damage);
      }
   }

 
}
