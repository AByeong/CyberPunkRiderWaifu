using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(GridGeneration))]
public class LightProbeAutoGenerator : MonoBehaviour
{
    [Header("Light Probe Settings")]
    [SerializeField] private float probeSpacing = 4f; // 프로브 간격
    [SerializeField] private float probeHeight = 2f; // 지면에서 프로브까지의 높이
    [SerializeField] private float ceilingHeight = 9f; // 천장의 높이
    [SerializeField] private float ceilingProbeOffset = 1f; // 천장에서 프로브까지의 오프셋
    [SerializeField] private bool generateFloorProbes = true; // 바닥 프로브 생성 여부
    [SerializeField] private bool generateCeilingProbes = true; // 천장 프로브 생성 여부
    [SerializeField] private bool generateWallProbes = true; // 벽 근처 프로브 생성 여부
    [SerializeField] private float wallProbeOffset = 1f; // 벽에서 프로브까지의 거리
    
    private GridGeneration gridGeneration;
    private LightProbeGroup lightProbeGroup;
    
    void Awake()
    {
        gridGeneration = GetComponent<GridGeneration>();
        
        // LightProbeGroup 컴포넌트가 없으면 추가
        lightProbeGroup = GetComponent<LightProbeGroup>();
        if (lightProbeGroup == null)
        {
            lightProbeGroup = gameObject.AddComponent<LightProbeGroup>();
        }
    }
    
    [ContextMenu("Generate Light Probes")]
    public void GenerateLightProbes()
    {
        if (gridGeneration == null)
        {
            gridGeneration = GetComponent<GridGeneration>();
            if (gridGeneration == null)
            {
                Debug.LogError("GridGeneration component not found!");
                return;
            }
        }
        
        if (lightProbeGroup == null)
        {
            lightProbeGroup = GetComponent<LightProbeGroup>();
            if (lightProbeGroup == null)
            {
                lightProbeGroup = gameObject.AddComponent<LightProbeGroup>();
            }
        }
        
        List<Vector3> probePositions = new List<Vector3>();
        
        // 그리드 크기와 오프셋 정보 가져오기
        int width = gridGeneration.width;
        int height = gridGeneration.height;
        float positionOffset = gridGeneration.PositionOffset;
        
        Debug.Log($"Generating light probes for map size: {width}x{height}, offset: {positionOffset}");
        
        // 그리드 정보를 가져오기 위해 리플렉션 사용 (private 필드 접근)
        var gridField = typeof(GridGeneration).GetField("grid", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (gridField == null)
        {
            Debug.LogError("Cannot access grid data from GridGeneration!");
            return;
        }
        
        int[,] grid = (int[,])gridField.GetValue(gridGeneration);
        
        if (grid == null)
        {
            Debug.LogError("Grid data is null! Make sure to generate the map first.");
            return;
        }
        
        // 프로브 간격에 따른 실제 스텝 계산
        int stepX = Mathf.Max(1, Mathf.RoundToInt(probeSpacing / positionOffset));
        int stepY = Mathf.Max(1, Mathf.RoundToInt(probeSpacing / positionOffset));
        
        Debug.Log($"Probe step: {stepX}, {stepY}");
        
        for (int x = 0; x < width; x += stepX)
        {
            for (int y = 0; y < height; y += stepY)
            {
                EGirdType gridType = (EGirdType)grid[x, y];
                
                // Blank 타입은 건너뛰기
                if (gridType == EGirdType.Blank)
                    continue;
                
                Vector3 worldPos = new Vector3(x * positionOffset, 0, y * positionOffset);
                
                // 바닥 프로브 생성
                if (generateFloorProbes && IsWalkableArea(gridType))
                {
                    Vector3 floorProbePos = new Vector3(worldPos.x, worldPos.y + probeHeight, worldPos.z);
                    probePositions.Add(floorProbePos);
                }
                
                // 천장 프로브 생성 (천장이 있는 곳에만)
                if (generateCeilingProbes && HasCeiling(gridType))
                {
                    // Inspector에서 설정한 천장 높이 사용
                    Vector3 ceilingProbePos = new Vector3(worldPos.x, worldPos.y + ceilingHeight - ceilingProbeOffset, worldPos.z);
                    probePositions.Add(ceilingProbePos);
                }
                
                // 벽 근처 프로브 생성
                if (generateWallProbes && gridType == EGirdType.Border)
                {
                    GenerateWallProbes(probePositions, worldPos, x, y, grid, width, height, positionOffset);
                }
            }
        }
        
        // 추가적으로 경로 연결부분에 더 세밀한 프로브 배치
        GeneratePathTransitionProbes(probePositions, grid, width, height, positionOffset);
        
        Debug.Log($"Total probe positions generated: {probePositions.Count}");
        
        // 디버깅: 원점 근처 프로브 확인
        for (int i = 0; i < probePositions.Count; i++)
        {
            if (Vector3.Distance(probePositions[i], Vector3.zero) < 1f)
            {
                Debug.LogWarning($"Probe at index {i} is too close to origin: {probePositions[i]}");
            }
        }
        
        // LightProbeGroup에 프로브 위치 적용
        if (probePositions.Count > 0)
        {
            lightProbeGroup.probePositions = probePositions.ToArray();
            Debug.Log($"Generated {probePositions.Count} light probes for the generated map.");
        }
        else
        {
            Debug.LogWarning("No probe positions generated!");
        }
        
        // 에디터에서 Scene 뷰 업데이트
        #if UNITY_EDITOR
        UnityEditor.SceneView.RepaintAll();
        #endif
    }
    
    private bool IsWalkableArea(EGirdType gridType)
    {
        return gridType == EGirdType.Activate || 
               gridType == EGirdType.Path || 
               gridType == EGirdType.Enrty || 
               gridType == EGirdType.Exit ||
               gridType == EGirdType.Pattern ||
               gridType == EGirdType.Spawner;
    }
    
    private bool HasCeiling(EGirdType gridType)
    {
        // Blank이 아닌 모든 타입에는 천장이 있다고 가정
        return gridType != EGirdType.Blank;
    }
    
    private void GenerateWallProbes(List<Vector3> probePositions, Vector3 wallPos, int x, int y, 
        int[,] grid, int width, int height, float positionOffset)
    {
        // 벽 주변의 4방향을 체크하여 빈 공간이 있는 방향으로 프로브 배치
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
        
        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];
            
            // 그리드 범위 체크
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                EGirdType neighborType = (EGirdType)grid[nx, ny];
                
                // 인접한 셀이 걸을 수 있는 공간이면 그 방향으로 프로브 배치
                if (IsWalkableArea(neighborType))
                {
                    Vector3 probeOffset = directions[i] * wallProbeOffset;
                    Vector3 wallProbePos = wallPos + probeOffset + Vector3.up * (probeHeight * 0.5f);
                    probePositions.Add(wallProbePos);
                }
            }
        }
    }
    
    private void GeneratePathTransitionProbes(List<Vector3> probePositions, int[,] grid, 
        int width, int height, float positionOffset)
    {
        // 경로와 다른 타입 간의 전환 지점에 추가 프로브 배치
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                EGirdType currentType = (EGirdType)grid[x, y];
                
                if (currentType == EGirdType.Path || currentType == EGirdType.Enrty || currentType == EGirdType.Exit)
                {
                    // 주변에 다른 타입의 walkable 영역이 있는지 체크
                    bool hasTransition = false;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            
                            int nx = x + dx;
                            int ny = y + dy;
                            
                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                EGirdType neighborType = (EGirdType)grid[nx, ny];
                                if (IsWalkableArea(neighborType) && neighborType != currentType)
                                {
                                    hasTransition = true;
                                    break;
                                }
                            }
                        }
                        if (hasTransition) break;
                    }
                    
                    if (hasTransition)
                    {
                        // 수정된 부분: transform.position 제거
                        Vector3 worldPos = new Vector3(x * positionOffset, 0, y * positionOffset);
                        Vector3 transitionProbePos = new Vector3(worldPos.x, worldPos.y + probeHeight * 1.5f, worldPos.z);
                        probePositions.Add(transitionProbePos);
                    }
                }
            }
        }
    }
    
    [ContextMenu("Clear Light Probes")]
    public void ClearLightProbes()
    {
        if (lightProbeGroup != null)
        {
            lightProbeGroup.probePositions = new Vector3[0];
            Debug.Log("Light probes cleared.");
            
            #if UNITY_EDITOR
            UnityEditor.SceneView.RepaintAll();
            #endif
        }
    }
    
    // 맵이 새로 생성될 때 자동으로 프로브도 생성
    public void OnMapGenerated()
    {
        // 약간의 지연 후 프로브 생성 (맵 생성 완료 대기)
        Invoke(nameof(GenerateLightProbes), 0.1f);
    }
}