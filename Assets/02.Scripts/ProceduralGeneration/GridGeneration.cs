using System.Text;
using UnityEngine;
using System.Collections.Generic;

public enum EGirdType
{
    Blank,
    Activate,
    Enrty,
    Exit,
    Path,
    Pattern,
    Border

    // 0: 비활성 그리드
    // 1: 활성 그리드
    // 2: 입구
    // 3: 출구
    // 4: 길
    // 5: 패턴
}

public class GridGeneration : MonoBehaviour
{
    public GameObject FencePrefab;
    public GameObject FenceDoorPrefab;
    public GameObject PathPrefab;
    public GameObject[] PropPrefabs;

    public float PositionOffset;


    public int width = 20;
    public int height = 20;
    public int entryCount = 2;
    public int exitCount = 2;
    public int maskMargin = 2; // 맵 가장자리에서 안쪽으로 마스킹할 너비
    public int pathProximityRadius = 5; // 경로에서 일정 거리 이상 떨어진 Activate 셀을 제거할 기준 거리
    public float PatternDensity = 0.1f;

    private int[,] grid;
    private List<Vector2Int> entries = new List<Vector2Int>();
    private List<Vector2Int> exits = new List<Vector2Int>();


    // 1. NxM크기의 2차원 배열 생성
    // 2. 경계 검출
    // 3. 입구/출구 생성
    // 4. Path 생성
    // 5. Path에서 일정 거리 이상 떨어져 있으며, 활성화타입 그리드인 경우 제거
    // 6. 경계 검출
    // 7. 빈 공간 너비에 따른 랜덤패턴 생성
    // 8. 패턴들 구현

    private void Start()
    {
        Generate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            DestroyMap();
            Generate();
        }
    }

    public void Generate()
    {
        grid = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = (int)EGirdType.Activate;
            }
        }

        entries.Clear();
        exits.Clear();

        MaskOuterEmptySpaces();
        DetectBoundary();

        PlaceEntries();
        PlaceExits();

        foreach (var entry in entries)
        {
            foreach (var exit in exits)
            {
                GeneratePath(entry, exit);
            }
        }
        RemoveFarActivateCells();
        DetectBoundary();
        GenerateRandomPatterns();
        BuildMap();
    }


    void MaskOuterEmptySpaces()
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


    void DetectBoundary()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == (int)EGirdType.Activate && HasEmptyNeighbor(x, y))
                {
                    grid[x, y] = (int)EGirdType.Border;
                }
            }
        }
    }

    bool HasEmptyNeighbor(int x, int y)
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

    void PlaceEntries()
    {
        List<Vector2Int> candidates = GetInnerBoundaryCandidates();
        for (int i = 0; i < entryCount && candidates.Count > 0; i++)
        {
            Vector2Int entry = candidates[Random.Range(0, candidates.Count)];
            grid[entry.x, entry.y] = (int)EGirdType.Enrty;
            entries.Add(entry);
            candidates.Remove(entry);
        }
    }

    void PlaceExits()
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
            exits.Add(exit);
            candidates.Remove(exit);
        }
    }

    List<Vector2Int> GetInnerBoundaryCandidates()
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

    void GeneratePath(Vector2Int start, Vector2Int end)
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

    List<Vector2Int> FindPath(Vector2Int from, Vector2Int to) // BFS 사용
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

    void RemoveFarActivateCells()
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
                if (grid[x, y] == (int)EGirdType.Activate)
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

    void GenerateRandomPatterns()
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

    private void DestroyMap()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void BuildMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                switch (grid[i, j])
                {
                    case (int)EGirdType.Border:
                        {
                            GameObject fence = Instantiate(FencePrefab, transform);
                            fence.transform.position = transform.position + new Vector3(i * PositionOffset, 0, j * PositionOffset);
                            fence.transform.rotation = GetFenceRotation(i, j);
                            break;
                        }

                    case (int)EGirdType.Enrty:
                        {
                            GameObject fencedoor = Instantiate(FenceDoorPrefab, transform);
                            fencedoor.transform.position = transform.position + new Vector3(i * PositionOffset, transform.position.y, j * PositionOffset);
                            fencedoor.transform.rotation = GetFenceDoorRotation(i, j);
                            break;
                        }

                    case (int)EGirdType.Exit:
                        {
                            GameObject fencedoor = Instantiate(FenceDoorPrefab, transform);
                            fencedoor.transform.position = transform.position + new Vector3(i * PositionOffset, transform.position.y, j * PositionOffset);
                            fencedoor.transform.rotation = GetFenceDoorRotation(i, j);
                            break;
                        }

                    case (int)EGirdType.Path:
                        {
                            GameObject path = Instantiate(PathPrefab, transform);
                            path.transform.position = transform.position + new Vector3(i * PositionOffset, transform.position.y, j * PositionOffset);
                            break;
                        }

                    case (int)EGirdType.Pattern:
                        {

                            GameObject prop = Instantiate(PropPrefabs[Random.Range(0, PropPrefabs.Length - 1)], transform);
                            prop.transform.position = transform.position + new Vector3(i * PositionOffset, transform.position.y, j * PositionOffset);
                            break;
                        }
                }
            }
        }
    }

    Quaternion GetFenceRotation(int x, int y)
    {
        // +X 방향 (오른쪽)을 바라보려면 Y축 90도 회전
        // -X 방향 (왼쪽)을 바라보려면 Y축 -90도 회전
        // +Z 방향 (위쪽)을 바라보려면 Y축 0도 회전
        // -Z 방향 (아래쪽)을 바라보려면 Y축 180도 회전

        // 왼쪽 이웃이 Blank -> 펜스는 오른쪽 (+X)을 바라봐야 함
        if (x - 1 >= 0 && grid[x - 1, y] == (int)EGirdType.Blank)
        {
            return Quaternion.Euler(-90, 90, 0);
        }
        // 오른쪽 이웃이 Blank -> 펜스는 왼쪽 (-X)을 바라봐야 함
        if (x + 1 < width && grid[x + 1, y] == (int)EGirdType.Blank)
        {
            return Quaternion.Euler(-90, -90, 0);
        }

        // 아래쪽 이웃이 Blank -> 펜스는 위쪽 (+Z)을 바라봐야 함
        if (y - 1 >= 0 && grid[x, y - 1] == (int)EGirdType.Blank)
        {
            return Quaternion.Euler(-90, 0, 0);
        }
        // 위쪽 이웃이 Blank -> 펜스는 아래쪽 (-Z)을 바라봐야 함
        if (y + 1 < height && grid[x, y + 1] == (int)EGirdType.Blank)
        {
            return Quaternion.Euler(-90, 180, 0);
        }

        // 만약 어떤 이유로 Blank 이웃을 찾지 못한다면 기본 회전을 반환
        Debug.LogWarning($"Border cell at ({x}, {y}) has no cardinal Blank neighbor. Defaulting to identity rotation.");
        return Quaternion.Euler(-90, 0, 0); // 기본 회전 (0,0,0)
    }
    
    Quaternion GetFenceDoorRotation(int x, int y)
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
        return Quaternion.Euler(0, 0, 0); // 기본 회전 (0,0,0)
    }
}
