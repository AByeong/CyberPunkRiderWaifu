using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Mathematics;

public class StageManager : MonoBehaviour
{
    public List<GridGeneration> SubStageList;
    public List<NavMeshSurface> NavmeshSurfaceList;
    public GameObject Player;
    public CinemachineCamera CinemachineCam;
    public GameObject ExitPortal;

    public CinemachineOrbitalFollow OrbitalFollow;

    public bool _isClear = false;
    private int _currentStageIndex;
    private int _previousStageIndex;
    public int _nextStageIndex;

    private Queue<List<GameObject>> _entryQueue = new Queue<List<GameObject>>();
    private Queue<List<GameObject>> _exitQueue = new Queue<List<GameObject>>();

    private void Start()
    {
        DeliveryManager.Instance.OnCompleteSector += CompleteSector;

        // _orbitalFollow = CinemachineCam.GetCinemachineComponent<CinemachineOrbitalFollow>();

        StageInitialize();
        _currentStageIndex = 0;
        _nextStageIndex = SubStageList.Count - 1;

        GameObject startPoint = SubStageList[_currentStageIndex].GetStartEntry();
        MovePlayerToStartPosition(startPoint);
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
        CharacterController playerController = Player.GetComponent<CharacterController>();
        playerController.enabled = false;
        Player.transform.position = startPoint.transform.position;
        Player.transform.rotation = Quaternion.Euler(0, startPoint.transform.eulerAngles.y, 0);
        playerController.enabled = true;
    }

    public void MoveNextStage()
    {
        if (_isClear)
        {
            _isClear = false;
            GameObject startEntry = SubStageList[_nextStageIndex].GetStartEntry();
            MovePlayerToStartPosition(startEntry);
            TeleportThroughPortal(startEntry);

            // TODO: 함수로 따로 뺴기
            _previousStageIndex = _currentStageIndex;
            _currentStageIndex = _nextStageIndex;
            _nextStageIndex = _previousStageIndex;

            GenerateSubStage(_nextStageIndex);
        }
    }

    public void CompleteSector()
    {
        Debug.LogWarning($"{gameObject.name}: 섹터 클리어!");
    }

    public void AddEntries(List<GameObject> entry)
    {
        _entryQueue.Enqueue(entry);
    }

    public void RemoveEntries()
    {
        if (_entryQueue.Count > 0)
        {
            _entryQueue.Dequeue();
        }
    }

    public void AddExits(List<GameObject> exit)
    {
        _exitQueue.Enqueue(exit);
    }

    public void RemoveExits()
    {
        if (_exitQueue.Count > 0)
        {
            _exitQueue.Dequeue();
        }
    }

    public void TeleportThroughPortal(GameObject startPoint)
    {
        Vector3 relativePos = Camera.main.transform.position - ExitPortal.transform.position;
        Vector3 newCamPos = relativePos + startPoint.transform.position;

        Quaternion relativeCamRot = Quaternion.Inverse(ExitPortal.transform.rotation) * Camera.main.transform.rotation;
        Quaternion newCamRot = relativeCamRot * startPoint.transform.rotation;
        // Quaternion newnewRota = Quaternion.Euler(Camera.main.transform.eulerAngles.x, newCamRot.eulerAngles.y, Camera.main.transform.eulerAngles.x);

        if (CinemachineCam.Follow == null)
        {
            CinemachineCam.Follow = Player.transform;
        }

        OrbitalFollow.enabled = false;
        CinemachineCam.OnTargetObjectWarped(CinemachineCam.Follow, newCamPos - Camera.main.transform.position);
        CinemachineCam.ForceCameraPosition(newCamPos, newCamRot);
        // CinemachineCam.ForceCameraPosition(newCamPos, newnewRota);
        OrbitalFollow.enabled = true;
    }
}
