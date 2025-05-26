using System;
using Drakkar.GameUtils;
using UnityEngine;

public class PlayerTrails : MonoBehaviour
{
   public DrakkarTrail Drakkar;

   private void Start()
   {
      Drakkar.Init();
   }

   public void DrakkarTrailStart()
   {
      Drakkar.Begin();
   }
   
   
   public void DrakkarTrailEnd()
   {
      Drakkar.End();
   }
}
