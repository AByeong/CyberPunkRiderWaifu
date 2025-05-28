using System;
using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;
using Unity.Cinemachine;
using System.Collections;

public class StageManager : MonoBehaviour
{
    public List<GridGeneration> SubStageList;
    public List<NavMeshSurface> NavmeshSurfaceList;

    public CinemachineCamera CinemachineCam;

    public GameObject Player;
    public GameObject ExitPortal;

    public float SpawnDistance;


    private CinemachineOrbitalFollow _orbitalFollow;
    private CharacterController _playerController;

    public bool _isClear = false;
    private int _currentStageIndex;
    private int _previousStageIndex;
    public int _nextStageIndex;

    // private Queue<List<GameObject>> _entryQueue = new Queue<List<GameObject>>();
    // private Queue<List<GameObject>> _exitQueue = new Queue<List<GameObject>>();

    

    private void Start()
    {
        DeliveryManager.Instance.OnCompleteSector += () =>
        {

            CompleteSector();
            Debug.Log("StageManager에서 DeliveryManager OnCompeteSector에 구독함");

        };

        _orbitalFollow = CinemachineCam.GetComponent<CinemachineOrbitalFollow>();
        _playerController = Player.GetComponent<CharacterController>();

        StageInitialize();
        _currentStageIndex = 0;
        _nextStageIndex = SubStageList.Count - 1;

        GameObject startPoint = SubStageList[_currentStageIndex].GetStartEntry();
        ExitPortal = startPoint;
        MovePlayerToStartPosition(startPoint);
        AddSpawners(_currentStageIndex);
        StartCoroutine(WaitForPool());

        _isClear = false;
    }

    IEnumerator WaitForPool()
    {
        yield return new WaitForSeconds(1f);
        
        EnemyManager.Instance.InitSpawn();
    }


    public void StageInitialize()
    {
        for (int i = 0; i < SubStageList.Count; i++)
        {
            GenerateSubStage(i);
        }
    }

    public void GenerateSubStage(int stageIndex)
    {

        if (DeliveryManager.Instance.CurrentSector == DeliveryManager.Instance.CompleteSector - 1)
        {
            CinemachineManager.Instance.BossAppear();
        }


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

    private void MovePlayerToStartPosition(GameObject startPoint)
    {
        _playerController.enabled = false;

        Player.transform.position = startPoint.transform.position + startPoint.transform.forward * SpawnDistance;
        Player.transform.rotation = Quaternion.Euler(0, startPoint.transform.eulerAngles.y, 0);

        _playerController.enabled = true;

        Vector3 relativePos = Camera.main.transform.position - ExitPortal.transform.position;
        Vector3 newCamPos = relativePos + startPoint.transform.position;

        _orbitalFollow.HorizontalAxis.Value = startPoint.transform.eulerAngles.y;
        CinemachineCam.OnTargetObjectWarped(CinemachineCam.Follow, newCamPos - CinemachineCam.transform.position);
    }

    public void CheckToMoveNextStage()
    {
        if (_isClear)
        {
            CinemachineManager.Instance.ShowElevatorChangeAnimation();

        }
    }

    public void MoveNextStage()
    {
        _isClear = false;

        AddSpawners(_nextStageIndex);
        EnemyManager.Instance.InitSpawn();

        GameObject startEntry = SubStageList[_nextStageIndex].GetStartEntry();
        MovePlayerToStartPosition(startEntry);

        // TODO: 함수로 따로 뺴기
        _previousStageIndex = _currentStageIndex;
        _currentStageIndex = _nextStageIndex;
        _nextStageIndex = _previousStageIndex;

        DeliveryManager.Instance.LoadNextSection();
        GenerateSubStage(_nextStageIndex);
    }

    public void CompleteSector()
    {
        Debug.LogWarning($"{gameObject.name}: 섹터 클리어!");
        _isClear = true;
    }

    private void AddSpawners(int stageIndex)
    {
        if (EnemyManager.Instance.NormalMonsterSpawners.Count > 0)
        {
            EnemyManager.Instance.NormalMonsterSpawners.Clear();
        }
        foreach (MonsterSpawner spawner in SubStageList[stageIndex].NormalSpawners)
        {
            EnemyManager.Instance.AddNormalSpwner(spawner);
        }

        if (EnemyManager.Instance.EliteMonsterSpawners.Count > 0)
        {
            EnemyManager.Instance.EliteMonsterSpawners.Clear();
        }
        foreach (MonsterSpawner spawner in SubStageList[stageIndex].EliteSpawners)
        {
            EnemyManager.Instance.AddEliteSpawner(spawner);
        }

        MonsterSpawner Bossspawner =SubStageList[stageIndex].BossSpawner;
        EnemyManager.Instance.AddBossSpawner(Bossspawner);
    }
}
