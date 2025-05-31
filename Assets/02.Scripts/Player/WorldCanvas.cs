using System;
using UnityEngine;

public class WorldCanvas : MonoBehaviour
{
   private void Awake()
   {
      GameManager.Instance.WorldCanvas = this.gameObject;
   }
}
