using UnityEngine;
using System.Collections.Generic;
using System.Text;

public enum EGirdType
{
    Blank,
    Activate,
    Enrty,
    Exit,
    Path,
    Pattern,
    Border,
    CornerBorder,
    Spawner

    // 0: 비활성 그리드
    // 1: 활성 그리드
    // 2: 입구
    // 3: 출구
    // 4: 길
    // 5: 패턴
    // 6: 경계
    // 7: 경계 코너
}

public enum SpawnerType
{
    Normal,
    Elite0,
    Elite1,
    Boss
}

public class GridGeneration : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject FencePrefab;
    public GameObject[] FencevariationPrefabs;
    public GameObject FenceCornerPrefab;
    public GameObject FenceDoorPrefab;
    public GameObject PathPrefab;
    public GameObject FloorPrefab;
    public GameObject CeilingPrefab;
    public GameObject[] PropPrefabs;

    [Header("[Minimap Prefabs]")]
    public GameObject GroundMiniMapPrefab;
    public GameObject PathMiniMapPrefab;


    [Header("Module Size Offset")]
    public float PositionOffset; // 에셋 크기
    
    [Header("Spawners")]
    public MonsterSpawner[] NormalSpawner;
    public MonsterSpawner[] EliteSpawner;
    public MonsterSpawner BossSpawner;
    public MonsterSpawner BossPhase2Spawner;


    [Header("Parmeters")]
    public int width = 20;
    public int height = 20;
    public int entryCount = 2;
    public int exitCount = 2;
    public int maskMargin = 2; // 맵 가장자리에서 안쪽으로 마스킹할 너비
    public int pathProximityRadius = 5; // 경로에서 일정 거리 이상 떨어진 Activate 셀을 제거할 기준 거리
    public float PatternDensity = 0.1f;
    public float WallVariationChance = 0.1f;

    public int NormalSpawnerCount;
    public int Elite0SpanwerCount;
    public int Elite1SpanwerCount;
    public int BossSpawnerCount;

    private int _normalSpawnerCreated = 0;
    private int _elite0SpawnerCreated = 0;
    private int _elite1SpawnerCreated = 0;
    private int _bossSpawnerCreated = 0;
    private int _totalSpawnerCount;

    private int[,] grid;
    private List<Vector2Int> _exitsPos = new List<Vector2Int>();
    private List<Vector2Int> _entriesPos = new List<Vector2Int>();

    [SerializeField] private List<GameObject> _exits = new List<GameObject>();
    [SerializeField] private List<GameObject> _entries = new List<GameObject>();

    private float[] Roations = { 0, 90, 180, 270 };


    // 1. NxM크기의 2차원 배열 생성
    // 2. 경계 검출
    // 3. 입구/출구 생성
    // 4. Path 생성
    // 5. Path에서 일정 거리 이상 떨어져 있으며, 활성화타입 그리드인 경우 제거
    // 6. 경계 검출
    // 7. 빈 공간 너비에 따른 랜덤패턴 생성
    // 8. 패턴들 구현

    private void Awake()
    {

    }

    private void DebugGrid()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                sb.Append(grid[i, j]);
            }
            sb.Append("\n");
        }

        Debug.Log(sb);
    }

    public void Generate()
    {
        Initialize();
        MaskOuterEmptySpaces();

        DetectBoundary();

        PlaceEntries();
        PlaceExits();

        foreach (var entry in _entriesPos)
        {
            foreach (var exit in _exitsPos)
            {
                GeneratePath(entry, exit);
            }
        }

        RemoveFarActivateCells();

        DetectBoundary();

        GenerateRandomSpawner();
        GenerateRandomPatterns();

        BuildMap();

        // DebugGrid();
    }


    public void DestroyMap()
    {     
        // 맵 오브젝트 모두 해제
        foreach (Transform child in gameObject.transform)
        {
            child.position = new Vector3(9999, 9999, 9999);
            Destroy(child.gameObject);
        }
    }

    private void Initialize()
    {
        _totalSpawnerCount = NormalSpawnerCount + Elite0SpanwerCount + Elite1SpanwerCount + BossSpawnerCount;
        _normalSpawnerCreated = 0;
        _elite0SpawnerCreated = 0;
        _elite1SpawnerCreated = 0;
        _bossSpawnerCreated = 0;

        _entriesPos.Clear();
        _entries.Clear();

        _exitsPos.Clear();
        _exits.Clear();

        grid = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = (int)EGirdType.Activate;
            }
        }
    }

    private void MaskOuterEmptySpaces()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // maskMargin 안쪽에 있는 셀만 Activate로 유지하고, 나머지는 Blank로 설정
                // 단, 이미 Entry, Exit, Path로 설정된 셀은 덮어쓰지 않음
                if (x < maskMargin || x >= width - maskMargin || y < maskMargin || y >= height - maskMargin)
                {
                    if (grid[x, y] != (int)EGirdType.Enrty && grid[x, y] != (int)EGirdType.Exit && grid[x, y] != (int)EGirdType.Path)
                    {
                        grid[x, y] = (int)EGirdType.Blank;
                    }
                }
            }
        }
    }


    private void DetectBoundary()
    {
        // 임시로 경계 셀을 저장할 리스트 (반복문 내에서 grid를 직접 수정하면 문제 발생 가능)
        List<Vector2Int> potentialBorderCells = new List<Vector2Int>();

        // 첫 번째 순회: Blank에 인접한 모든 Activate 셀을 잠재적 경계 셀로 식별
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == (int)EGirdType.Border)
                {
                    potentialBorderCells.Add(new Vector2Int(x, y));
                    continue;
                }

                if (grid[x, y] == (int)EGirdType.Activate && HasEmptyNeighbor(x, y))
                {
                    potentialBorderCells.Add(new Vector2Int(x, y));
                }
            }
        }

        // 두 번째 순회: 잠재적 경계 셀을 Border 또는 CornerBorder로 최종 할당
        foreach (var cell in potentialBorderCells)
        {
            // 입구, 출구, 길은 Border/CornerBorder로 덮어쓰지 않음
            if (grid[cell.x, cell.y] != (int)EGirdType.Enrty &&
                grid[cell.x, cell.y] != (int)EGirdType.Exit &&
                grid[cell.x, cell.y] != (int)EGirdType.Path)
            {
                grid[cell.x, cell.y] = (int)EGirdType.Border;
            }
        }

        foreach (var cell in potentialBorderCells)
        {
            if (IsCornerBorder(cell.x, cell.y))
            {
                grid[cell.x, cell.y] = (int)EGirdType.CornerBorder;
            }
        }
    }

    private bool IsCornerBorder(int x, int y)
    {
        bool borderNorth =  (grid[x, y + 1] == (int)EGirdType.Border || grid[x, y + 1] == (int)EGirdType.CornerBorder || grid[x, y + 1] == (int)EGirdType.Enrty || grid[x, y + 1] == (int)EGirdType.Exit);
        bool borderSouth =  (grid[x, y - 1] == (int)EGirdType.Border || grid[x, y - 1] == (int)EGirdType.CornerBorder || grid[x, y - 1] == (int)EGirdType.Enrty || grid[x, y - 1] == (int)EGirdType.Exit);
        bool borderEast =   (grid[x + 1, y] == (int)EGirdType.Border || grid[x + 1, y] == (int)EGirdType.CornerBorder || grid[x + 1, y] == (int)EGirdType.Enrty || grid[x + 1, y] == (int)EGirdType.Exit);
        bool borderWest =   (grid[x - 1, y] == (int)EGirdType.Border || grid[x - 1, y] == (int)EGirdType.CornerBorder || grid[x - 1, y] == (int)EGirdType.Enrty || grid[x - 1, y] == (int)EGirdType.Exit);

        if (borderNorth && borderWest) // 북서 코너 (맵의 안쪽은 남동쪽)
        {
            return true; // 0도 회전 (기본 방향)
        }
        else if (borderNorth && borderEast) // 북동 코너 (맵의 안쪽은 남서쪽)
        {
            return true; // 90도 회전
        }
        else if (borderSouth && borderEast) // 남동 코너 (맵의 안쪽은 북서쪽)
        {
            return true; // 180도 회전
        }
        else if (borderSouth && borderWest) // 남서 코너 (맵의 안쪽은 북동쪽)
        {
            return true; // 270도 회전
        }

        return false;
    }

    private bool HasEmptyNeighbor(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;
                if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                {
                    return true;
                }
                else
                {
                    if (grid[nx, ny] == (int)EGirdType.Blank)
                        return true;
                }
            }
        }
        return false;
    }

    private void PlaceEntries()
    {
        List<Vector2Int> candidates = GetInnerBoundaryCandidates();
        for (int i = 0; i < entryCount && candidates.Count > 0; i++)
        {
            Vector2Int entry = candidates[Random.Range(0, candidates.Count)];
            grid[entry.x, entry.y] = (int)EGirdType.Enrty;
            _entriesPos.Add(entry);
            candidates.Remove(entry);
        }
    }

    private void PlaceExits()
    {
        List<Vector2Int> candidates = GetInnerBoundaryCandidates();
        for (int i = 0; i < exitCount && candidates.Count > 0; i++)
        {
            Vector2Int exit = candidates[Random.Range(0, candidates.Count)];
            while (grid[exit.x, exit.y] == (int)EGirdType.Enrty && candidates.Count > 1)
            {
                exit = candidates[Random.Range(0, candidates.Count)];
            }

            grid[exit.x, exit.y] = (int)EGirdType.Exit;
            _exitsPos.Add(exit);
            candidates.Remove(exit);
        }
    }

    private List<Vector2Int> GetInnerBoundaryCandidates()
    {
        List<Vector2Int> candidates = new List<Vector2Int>();

        // 상단 및 하단 내부 경계
        for (int x = maskMargin; x < width - maskMargin; x++)
        {
            // 상단 내부 경계 (y = maskMargin)
            if (grid[x, maskMargin] == (int)EGirdType.Activate || grid[x, maskMargin] == (int)EGirdType.Border)
            {
                candidates.Add(new Vector2Int(x, maskMargin));
            }
            // 하단 내부 경계 (y = height - 1 - maskMargin)
            if (grid[x, height - 1 - maskMargin] == (int)EGirdType.Activate || grid[x, height - 1 - maskMargin] == (int)EGirdType.Border)
            {
                candidates.Add(new Vector2Int(x, height - 1 - maskMargin));
            }
        }

        // 좌측 및 우측 내부 경계 (코너 중복 방지를 위해 y 범위 조정)
        for (int y = maskMargin + 1; y < height - 1 - maskMargin; y++)
        {
            // 좌측 내부 경계 (x = maskMargin)
            if (grid[maskMargin, y] == (int)EGirdType.Activate || grid[maskMargin, y] == (int)EGirdType.Border)
            {
                candidates.Add(new Vector2Int(maskMargin, y));
            }
            // 우측 내부 경계 (x = width - 1 - maskMargin)
            if (grid[width - 1 - maskMargin, y] == (int)EGirdType.Activate || grid[width - 1 - maskMargin, y] == (int)EGirdType.Border)
            {
                candidates.Add(new Vector2Int(width - 1 - maskMargin, y));
            }
        }
        return candidates;
    }

    private void GeneratePath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> pathPoints;
        if (Random.value < 0.5f)
        {
            // 최단거리
            pathPoints = FindPath(start, end);
        }
        else
        {
            // 중앙점을 경유하는 최단거리
            Vector2Int center = new Vector2Int(width / 2, height / 2);
            pathPoints = new List<Vector2Int>();
            pathPoints.AddRange(FindPath(start, center));
            pathPoints.AddRange(FindPath(center, end));
        }


        foreach (var p in pathPoints)
        {
            // 입구 또는 출구 지점을 덮어쓰지 않도록 확인
            if (grid[p.x, p.y] != (int)EGirdType.Enrty && grid[p.x, p.y] != (int)EGirdType.Exit)
            {
                grid[p.x, p.y] = (int)EGirdType.Path;
            }
        }
    }

    private List<Vector2Int> FindPath(Vector2Int from, Vector2Int to) // BFS 사용
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(from);
        visited.Add(from);

        int[] dx = { 0, 0, 1, -1 }; // 4방향 (상, 하, 우, 좌)
        int[] dy = { 1, -1, 0, 0 };

        bool pathFound = false;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == to)
            {
                pathFound = true;
                break;
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2Int neighbor = new Vector2Int(current.x + dx[i], current.y + dy[i]);

                // 그리드 범위 체크
                if (neighbor.x < 0 || neighbor.y < 0 || neighbor.x >= width || neighbor.y >= height)
                    continue;

                // Blank 및 Border 셀은 피합니다.
                // Entry/Exit 셀은 경로의 시작/끝이므로 피하지 않습니다.
                if (grid[neighbor.x, neighbor.y] == (int)EGirdType.Blank ||
                    grid[neighbor.x, neighbor.y] == (int)EGirdType.Border)
                    continue;

                // 이미 방문한 셀은 피합니다.
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        List<Vector2Int> path = new List<Vector2Int>();
        if (pathFound)
        {
            Vector2Int current = to;
            // 목표 지점부터 시작 지점까지 역추적하여 경로를 구성합니다.
            while (current != from)
            {
                // 목표 지점(Exit)은 경로에 포함하지 않습니다. (Exit 타입 유지)
                if (current != to)
                {
                    path.Insert(0, current); // 경로의 시작 부분에 삽입하여 순서 유지
                }
                if (!cameFrom.ContainsKey(current))
                {
                    Debug.LogError("Path reconstruction failed: 'cameFrom' does not contain current cell.");
                    path.Clear(); // 유효하지 않은 경로 제거
                    break;
                }
                current = cameFrom[current];
            }
            // 시작 지점(Entry)도 경로에 포함하지 않습니다. (Entry 타입 유지)
        }
        else
        {
            Debug.LogWarning($"No path found from {from} to {to}. This might lead to isolated areas.");
        }

        return path;
    }

    private void RemoveFarActivateCells()
    {
        // 경로, 입구, 출구 셀들을 빠르게 찾기 위한 리스트
        List<Vector2Int> currentPathAndDoorCells = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == (int)EGirdType.Path || grid[x, y] == (int)EGirdType.Enrty || grid[x, y] == (int)EGirdType.Exit)
                {
                    currentPathAndDoorCells.Add(new Vector2Int(x, y));
                }
            }
        }

        // 제거할 셀들을 임시로 저장할 리스트
        List<Vector2Int> cellsToRemove = new List<Vector2Int>();

        // maskMargin 안쪽의 Activate 셀들만 검사하여 경로와 너무 멀리 떨어진 셀을 찾습니다.
        for (int x = maskMargin; x < width - maskMargin; x++)
        {
            for (int y = maskMargin; y < height - maskMargin; y++)
            {
                // 현재 셀이 Activate 타입이고, 이미 Border가 아닌 경우에만 검사
                if (grid[x, y] == (int)EGirdType.Activate || grid[x, y] == (int)EGirdType.Border || grid[x, y] == (int)EGirdType.CornerBorder)
                {
                    bool isNearPath = false;
                    foreach (var pathCell in currentPathAndDoorCells)
                    {
                        // 현재 Activate 셀과 모든 경로/입구/출구 셀 간의 거리를 계산
                        if (Vector2Int.Distance(new Vector2Int(x, y), pathCell) < pathProximityRadius)
                        {
                            isNearPath = true;
                            break; // 가까운 경로 셀을 찾았으므로 더 이상 검사할 필요 없음
                        }
                    }

                    if (!isNearPath)
                    {
                        cellsToRemove.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        // 제거할 셀들을 Blank로 변경
        foreach (var cell in cellsToRemove)
        {
            grid[cell.x, cell.y] = (int)EGirdType.Blank;
        }
    }

    private void GenerateRandomSpawner()
    {
        while (_totalSpawnerCount > 0)
        {
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (grid[x, y] == (int)EGirdType.Activate && Random.value < PatternDensity)
                    {
                        grid[x, y] = (int)EGirdType.Spawner;
                        --_totalSpawnerCount;
                    }
                }
            }
        }
    }

    private void GenerateRandomPatterns()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (grid[x, y] == (int)EGirdType.Activate && Random.value < PatternDensity)
                {
                    grid[x, y] = (int)EGirdType.Pattern;
                }
            }
        }
    }

    private void BuildMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (grid[i, j] != (int)EGirdType.Blank)
                {
                    GameObject ceiling = Instantiate(CeilingPrefab, transform);
                    ceiling.transform.position = transform.position + new Vector3(i * PositionOffset, 0, j * PositionOffset);

                    if (grid[i, j] == (int)EGirdType.Path)
                    {
                        GameObject pathMinimap = Instantiate(PathMiniMapPrefab, transform);
                        pathMinimap.transform.position = transform.position + new Vector3(i * PositionOffset, 0, j * PositionOffset);
                    }
                    else
                    {
                        GameObject groundMinimap = Instantiate(GroundMiniMapPrefab, transform);
                        groundMinimap.transform.position = transform.position + new Vector3(i * PositionOffset, 0, j * PositionOffset);
                    }
                }

                if (grid[i, j] == (int)EGirdType.Activate || grid[i, j] == (int)EGirdType.Pattern || grid[i, j] == (int)EGirdType.Spawner || grid[i, j] == (int)EGirdType.CornerBorder)
                {
                    GameObject floor = Instantiate(FloorPrefab, transform);
                    floor.transform.position = transform.position + new Vector3(i * PositionOffset, 0, j * PositionOffset);
                }

                switch (grid[i, j])
                    {
                        case (int)EGirdType.Border:
                            {
                                GameObject fence = null;
                                if (Random.Range(0f, 1f) <= WallVariationChance)
                                {
                                    fence = Instantiate(FencevariationPrefabs[Random.Range(0, FencevariationPrefabs.Length)], transform);
                                }
                                else
                                {
                                    fence = Instantiate(FencePrefab, transform);
                                }
                                fence.transform.position = transform.position + new Vector3(i * PositionOffset, 0, j * PositionOffset);
                                fence.transform.rotation = GetFenceRotation(i, j);
                                
                                break;
                            }

                        case (int)EGirdType.CornerBorder:
                            {
                                GameObject fenceCorenr = Instantiate(FenceCornerPrefab, transform);
                                fenceCorenr.transform.position = transform.position + new Vector3(i * PositionOffset, 0, j * PositionOffset);
                                fenceCorenr.transform.rotation = GetCornerFenceRotation(i, j);
                                break;
                            }

                        case (int)EGirdType.Enrty:
                            {
                                GameObject fencedoor = Instantiate(FenceDoorPrefab, transform);
                                fencedoor.transform.position = transform.position + new Vector3(i * PositionOffset, transform.position.y, j * PositionOffset);
                                fencedoor.transform.rotation = GetFenceDoorRotation(i, j);
                                _entries.Add(fencedoor);

                                GameObject path = Instantiate(PathPrefab, transform);
                                path.transform.position = transform.position + new Vector3(i * PositionOffset, transform.position.y, j * PositionOffset);
                                break;
                            }

                        case (int)EGirdType.Exit:
                            {
                                GameObject fencedoor = Instantiate(FenceDoorPrefab, transform);
                                fencedoor.transform.position = transform.position + new Vector3(i * PositionOffset, transform.position.y, j * PositionOffset);
                                fencedoor.transform.rotation = GetFenceDoorRotation(i, j);
                                _exits.Add(fencedoor);

                                GameObject path = Instantiate(PathPrefab, transform);
                                path.transform.position = transform.position + new Vector3(i * PositionOffset, transform.position.y, j * PositionOffset);
                                break;
                            }

                        case (int)EGirdType.Path:
                            {
                                GameObject path = Instantiate(PathPrefab, transform);
                                path.transform.position = transform.position + new Vector3(i * PositionOffset, transform.position.y, j * PositionOffset);
                                break;
                            }

                        case (int)EGirdType.Spawner:
                            {
                                var result = GetNextSpawnerPrefab();
                                if (result.HasValue)
                                {
                                    var (spawner, type) = result.Value;
                                    spawner.transform.position = transform.position + new Vector3(i * PositionOffset, transform.position.y, j * PositionOffset);
                                }
                                break;
                            }

                        case (int)EGirdType.Pattern:
                            {

                                GameObject prop = Instantiate(PropPrefabs[Random.Range(0, PropPrefabs.Length)], transform);
                                prop.transform.position = transform.position + new Vector3(i * PositionOffset, transform.position.y, j * PositionOffset);
                                prop.transform.eulerAngles = new Vector3(0, Roations[Random.Range(0, Roations.Length)], 0);
                                break;
                            }
                    }
            }
        }
    }

    private (MonsterSpawner prefab, SpawnerType type)? GetNextSpawnerPrefab()
    {
        if (_normalSpawnerCreated < NormalSpawnerCount)
        {
            _normalSpawnerCreated++;
            return (NormalSpawner[0], SpawnerType.Normal);
        }
        else if (_elite0SpawnerCreated < Elite0SpanwerCount)
        {
            _elite0SpawnerCreated++;
            return (EliteSpawner[0], SpawnerType.Elite0);
        }
        else if (_elite1SpawnerCreated < Elite1SpanwerCount)
        {
            _elite1SpawnerCreated++;
            return (EliteSpawner[1], SpawnerType.Elite1);
        }
        else if (_bossSpawnerCreated < BossSpawnerCount)
        {
            _bossSpawnerCreated++;
            return (BossSpawner, SpawnerType.Boss);
        }

        return null;
    }

    private Quaternion GetFenceRotation(int x, int y)
    {
        // +X 방향 (오른쪽)을 바라보려면 Y축 90도 회전
        // -X 방향 (왼쪽)을 바라보려면 Y축 -90도 회전
        // +Z 방향 (위쪽)을 바라보려면 Y축 0도 회전
        // -Z 방향 (아래쪽)을 바라보려면 Y축 180도 회전

        // 왼쪽 이웃이 Blank -> 펜스는 오른쪽 (+X)을 바라봐야 함
        if (x - 1 >= 0 && grid[x - 1, y] == (int)EGirdType.Blank)
        {
            return Quaternion.Euler(0, 90, 0);
        }
        // 오른쪽 이웃이 Blank -> 펜스는 왼쪽 (-X)을 바라봐야 함
        if (x + 1 < width && grid[x + 1, y] == (int)EGirdType.Blank)
        {
            return Quaternion.Euler(0, -90, 0);
        }

        // 아래쪽 이웃이 Blank -> 펜스는 위쪽 (+Z)을 바라봐야 함
        if (y - 1 >= 0 && grid[x, y - 1] == (int)EGirdType.Blank)
        {
            return Quaternion.Euler(0, 0, 0);
        }
        // 위쪽 이웃이 Blank -> 펜스는 아래쪽 (-Z)을 바라봐야 함
        if (y + 1 < height && grid[x, y + 1] == (int)EGirdType.Blank)
        {
            return Quaternion.Euler(0, 180, 0);
        }

        // 만약 어떤 이유로 Blank 이웃을 찾지 못한다면 기본 회전을 반환
        Debug.LogWarning($"Border cell at ({x}, {y}) has no cardinal Blank neighbor. Defaulting to identity rotation.");
        return Quaternion.Euler(-90, 0, 0); // 기본 회전 (0,0,0)
    }

    private Quaternion GetCornerFenceRotation(int x, int y)
    {
        bool borderNorth = (grid[x, y + 1] == (int)EGirdType.Border || grid[x, y + 1] == (int)EGirdType.CornerBorder || grid[x, y + 1] == (int)EGirdType.Enrty || grid[x, y + 1] == (int)EGirdType.Exit);
        bool borderSouth = (grid[x, y - 1] == (int)EGirdType.Border || grid[x, y - 1] == (int)EGirdType.CornerBorder || grid[x, y - 1] == (int)EGirdType.Enrty || grid[x, y - 1] == (int)EGirdType.Exit);
        bool borderEast = (grid[x + 1, y] == (int)EGirdType.Border || grid[x + 1, y] == (int)EGirdType.CornerBorder || grid[x + 1, y] == (int)EGirdType.Enrty || grid[x + 1, y] == (int)EGirdType.Exit);
        bool borderWest = (grid[x - 1, y] == (int)EGirdType.Border || grid[x - 1, y] == (int)EGirdType.CornerBorder || grid[x - 1, y] == (int)EGirdType.Enrty || grid[x - 1, y] == (int)EGirdType.Exit);

        if (borderNorth && borderWest) // 북서 코너 (맵의 안쪽은 남동쪽)
        {
            return Quaternion.Euler(0, 0, 0); // 0도 회전 (기본 방향)
        }
        else if (borderNorth && borderEast) // 북동 코너 (맵의 안쪽은 남서쪽)
        {
            return Quaternion.Euler(0, 90, 0); // 90도 회전
        }
        else if (borderSouth && borderEast) // 남동 코너 (맵의 안쪽은 북서쪽)
        {
            return Quaternion.Euler(0, 180, 0); // 180도 회전
        }
        else if (borderSouth && borderWest) // 남서 코너 (맵의 안쪽은 북동쪽)
        {
            return Quaternion.Euler(0, 270, 0); // 270도 회전
        }

        return Quaternion.Euler(-90, 0, 0);
    }

    private Quaternion GetFenceDoorRotation(int x, int y)
    {
        // +X 방향 (오른쪽)을 바라보려면 Y축 90도 회전
        // -X 방향 (왼쪽)을 바라보려면 Y축 -90도 회전
        // +Z 방향 (위쪽)을 바라보려면 Y축 0도 회전
        // -Z 방향 (아래쪽)을 바라보려면 Y축 180도 회전

        // 왼쪽 이웃이 Blank -> 펜스는 오른쪽 (+X)을 바라봐야 함
        if (x - 1 >= 0 && grid[x - 1, y] == (int)EGirdType.Blank)
        {
            return Quaternion.Euler(0, 90, 0);
        }
        // 오른쪽 이웃이 Blank -> 펜스는 왼쪽 (-X)을 바라봐야 함
        if (x + 1 < width && grid[x + 1, y] == (int)EGirdType.Blank)
        {
            return Quaternion.Euler(0, -90, 0);
        }

        // 아래쪽 이웃이 Blank -> 펜스는 위쪽 (+Z)을 바라봐야 함
        if (y - 1 >= 0 && grid[x, y - 1] == (int)EGirdType.Blank)
        {
            return Quaternion.Euler(0, 0, 0);
        }
        // 위쪽 이웃이 Blank -> 펜스는 아래쪽 (-Z)을 바라봐야 함
        if (y + 1 < height && grid[x, y + 1] == (int)EGirdType.Blank)
        {
            return Quaternion.Euler(0, 180, 0);
        }

        // 만약 어떤 이유로 Blank 이웃을 찾지 못한다면 기본 회전을 반환
        Debug.LogWarning($"Border cell at ({x}, {y}) has no cardinal Blank neighbor. Defaulting to identity rotation.");
        return Quaternion.Euler(-90, 0, 0); // 기본 회전 (0,0,0)
    }

    public GameObject GetStartEntry()
    {
        int randomIndex = Random.Range(0, entryCount - 1);
        return _entries[randomIndex];
    }

}
