using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class StageBaker : MonoBehaviour
{
   public NavMeshSurface NavMeshSurface;

   public void BakeNavemesh()
   {
      NavMeshSurface.BuildNavMesh();
   }
   
   
}
