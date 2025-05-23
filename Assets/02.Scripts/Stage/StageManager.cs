using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class StageManager : Singleton<MonoBehaviour>
{
    public List<GridGeneration> SubStageList;
    public List<NavMeshSurface> NavmeshSurfaceList;
    public GameObject Player;

    private bool _isClear = false;
    private int _currentStageIndex;
    private int _previousStageIndex;
    private int _nextStageIndex;

    private void Start()
    {
        StageInitialize();
        _currentStageIndex = 0;
        _nextStageIndex = SubStageList.Count - 1;
        MovePlayerToStartPosition(_currentStageIndex);
    }

    // 디버깅
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            StageInitialize();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            _isClear = true;
            MoveNextStage();
        }
        _isClear = false;
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

    private void MovePlayerToStartPosition(int stageIndex)
    {
        Vector3 startPosition = SubStageList[stageIndex].transform.worldToLocalMatrix * SubStageList[stageIndex].GetStartPos();
        CharacterController playerController = Player.GetComponent<CharacterController>();
        playerController.enabled = false;
        Player.transform.position = startPosition;
        playerController.enabled = true;
    }

    public void MoveNextStage()
    {
        if (_isClear)
        {
            MovePlayerToStartPosition(_nextStageIndex);

            // TODO: 함수로 따로 뺴기
            _previousStageIndex = _currentStageIndex;
            _currentStageIndex = _nextStageIndex;
            _nextStageIndex = _previousStageIndex;

            GenerateSubStage(_nextStageIndex);
        }

    }
}
