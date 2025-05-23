using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class StageManager : Singleton<MonoBehaviour>
{
    public List<GridGeneration> SubStageList;
    public List<NavMeshSurface> NavmeshSurfaceList;

    private void Start()
    {
        StageInitialize();
    }

    // 디버깅
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            StageInitialize();
        }
    }
    // 디버깅

    public void StageInitialize()
    {
        for (int i = 0; i < SubStageList.Count; i++)
        {
            GenerateSubStage(i);
        }
    }

    public void GenerateSubStage(int stageIndex)
    {
        if (SubStageList.Count == 0 || SubStageList == null)
        {
            Debug.LogError($"{gameObject.name}: SubStageList가 비어있습니다!!");
            return;
        }

        if (NavmeshSurfaceList.Count == 0 || NavmeshSurfaceList == null)
        {
            Debug.LogError($"{gameObject.name}: NavmeshSurfaceList가 비어있습니다!!");
            return;
        }

        if (stageIndex > SubStageList.Count)
        {
            Debug.LogError($"{gameObject.name} : {stageIndex}는 유효하지 않은 인덱스입니다!!");
            return;
        }

        GridGeneration subStage = SubStageList[stageIndex];

        if (subStage.transform.childCount > 0)
        {
            subStage.DestroyMap();
        }
        subStage.Generate();

        NavmeshSurfaceList[stageIndex].UpdateNavMesh(NavmeshSurfaceList[stageIndex].navMeshData);
    }
}
