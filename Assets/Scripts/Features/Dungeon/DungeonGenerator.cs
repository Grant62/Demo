using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Dungeon
{
    public class DungeonGenerator : MonoBehaviour
    {
    [Header("Grid Settings")]
    public int gridWidth = 7;
    public int gridHeight = 7;
    public float roomSize = 60f;
    public float spacing = 120f;

    [Header("Prefabs")]
    public GameObject roomPrefab;
    public GameObject linePrefab;

    [Header("Room Count")]
    public int minRoomCount = 6;
    public int maxRoomCount = 20;

    [Header("Generation")]
    [Range(0f, 1f)]
    public float branchProbability = 0.3f;
    public int maxBranchDepth = 2;
    public int maxBranchCount = 4;
    public int maxRetries = 20;

    private bool[,] _grid;
    private Room[,] _rooms;
    private Canvas _canvas;
    private RectTransform _roomsContainer;
    private RectTransform _linesContainer;
    private Vector2Int _entrancePos;
    private Vector2Int _exitPos;

    private static readonly Vector2Int[] Directions =
    {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0)
    };

    private void Start()
    {
        Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        ClearGenerated();
        EnsureCanvas();

        bool valid = false;
        int roomCount = 0;

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            _grid = new bool[gridWidth, gridHeight];
            _rooms = new Room[gridWidth, gridHeight];

            PickEntranceAndExit();
            GenerateMainPath();
            AddBranches();

            roomCount = CountRooms();

            if (roomCount >= minRoomCount && roomCount <= maxRoomCount)
            {
                valid = true;
                break;
            }

            if (roomCount < minRoomCount)
            {
                FillToMin();
                roomCount = CountRooms();
                if (roomCount >= minRoomCount && roomCount <= maxRoomCount)
                {
                    valid = true;
                    break;
                }
            }
        }

        if (!valid)
        {
            _grid = new bool[gridWidth, gridHeight];
            _rooms = new Room[gridWidth, gridHeight];
            PickEntranceAndExit();
            GenerateMainPath();
            FillToMin();
            roomCount = CountRooms();
            Debug.LogWarning($"[Dungeon] Could not meet room count after {maxRetries} retries. Generated {roomCount} rooms.");
        }

        CreateRoomObjects();
        CreateConnections();
    }

    public void ClearGenerated()
    {
        var existing = GetComponentInChildren<DungeonRoot>();
        if (existing != null)
            DestroyImmediate(existing.gameObject);
    }

    private void EnsureCanvas()
    {
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            var canvasGO = new GameObject("DungeonCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGO.transform.SetParent(transform.parent ?? transform);
            _canvas = canvasGO.GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = _canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
            esGO.transform.SetParent(_canvas.transform.parent ?? transform);
        }

        var rootGO = new GameObject("DungeonRoot", typeof(RectTransform), typeof(DungeonRoot));
        rootGO.transform.SetParent(_canvas.transform, false);
        var rootRT = rootGO.GetComponent<RectTransform>();
        rootRT.anchorMin = Vector2.zero;
        rootRT.anchorMax = Vector2.one;
        rootRT.offsetMin = Vector2.zero;
        rootRT.offsetMax = Vector2.zero;

        var linesGO = new GameObject("Lines", typeof(RectTransform));
        linesGO.transform.SetParent(rootGO.transform, false);
        _linesContainer = linesGO.GetComponent<RectTransform>();

        var roomsGO = new GameObject("Rooms", typeof(RectTransform));
        roomsGO.transform.SetParent(rootGO.transform, false);
        _roomsContainer = roomsGO.GetComponent<RectTransform>();
    }

    private void PickEntranceAndExit()
    {
        int entranceEdge = Random.Range(0, 2);
        if (entranceEdge == 0)
            _entrancePos = new Vector2Int(0, Random.Range(0, gridHeight));
        else
            _entrancePos = new Vector2Int(Random.Range(0, gridWidth), 0);

        int exitEdge = Random.Range(0, 2);
        if (exitEdge == 0)
            _exitPos = new Vector2Int(gridWidth - 1, Random.Range(0, gridHeight));
        else
            _exitPos = new Vector2Int(Random.Range(0, gridWidth), gridHeight - 1);

        if (_entrancePos == _exitPos)
        {
            _exitPos.x = Mathf.Min(_exitPos.x + 1, gridWidth - 1);
            _exitPos.y = Mathf.Min(_exitPos.y + 1, gridHeight - 1);
        }
    }

    private void GenerateMainPath()
    {
        bool[,] visited = new bool[gridWidth, gridHeight];
        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(_entrancePos);
        visited[_entrancePos.x, _entrancePos.y] = true;

        Vector2Int current = _entrancePos;
        int maxSteps = gridWidth * gridHeight * 3;

        for (int i = 0; i < maxSteps && current != _exitPos; i++)
        {
            var candidates = GetUnvisitedNeighbors(current, visited);
            if (candidates.Count == 0) break;

            float totalWeight = 0f;
            float[] weights = new float[candidates.Count];
            for (int j = 0; j < candidates.Count; j++)
            {
                float distBefore = Vector2Int.Distance(current, _exitPos);
                float distAfter = Vector2Int.Distance(candidates[j], _exitPos);
                float improvement = distBefore - distAfter;
                weights[j] = Mathf.Max(0.1f, 1f + improvement * 5f);
                totalWeight += weights[j];
            }

            float r = Random.Range(0f, totalWeight);
            float cumulative = 0f;
            int chosen = 0;
            for (int j = 0; j < weights.Length; j++)
            {
                cumulative += weights[j];
                if (r <= cumulative) { chosen = j; break; }
            }

            current = candidates[chosen];
            path.Add(current);
            visited[current.x, current.y] = true;
        }

        if (current != _exitPos)
        {
            ForceConnectToExit(path, visited);
        }

        foreach (var pos in path)
            _grid[pos.x, pos.y] = true;
    }

    private void ForceConnectToExit(List<Vector2Int> path, bool[,] visited)
    {
        Vector2Int from = path[path.Count - 1];
        Vector2Int cursor = from;

        while (cursor != _exitPos)
        {
            int dx = Mathf.Clamp(_exitPos.x - cursor.x, -1, 1);
            int dy = Mathf.Clamp(_exitPos.y - cursor.y, -1, 1);

            if (dx != 0 && dy != 0)
            {
                if (Random.Range(0f, 1f) < 0.5f) dy = 0;
                else dx = 0;
            }

            Vector2Int step = new Vector2Int(cursor.x + dx, cursor.y + dy);
            if (!IsInBounds(step)) break;
            if (!visited[step.x, step.y])
            {
                cursor = step;
                path.Add(cursor);
                visited[cursor.x, cursor.y] = true;
            }
            else
            {
                if (dx != 0 && IsInBounds(new Vector2Int(cursor.x + dx, cursor.y)) && !visited[cursor.x + dx, cursor.y])
                    cursor = new Vector2Int(cursor.x + dx, cursor.y);
                else if (dy != 0 && IsInBounds(new Vector2Int(cursor.x, cursor.y + dy)) && !visited[cursor.x, cursor.y + dy])
                    cursor = new Vector2Int(cursor.x, cursor.y + dy);
                else
                    break;
            }
        }
    }

    private void AddBranches()
    {
        List<Vector2Int> mainPathRooms = new List<Vector2Int>();
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
                if (_grid[x, y])
                    mainPathRooms.Add(new Vector2Int(x, y));

        int branchesAdded = 0;
        foreach (var room in mainPathRooms)
        {
            if (room == _entrancePos || room == _exitPos) continue;
            if (branchesAdded >= maxBranchCount) break;
            if (Random.Range(0f, 1f) > branchProbability) continue;

            var dirs = new List<Vector2Int>(Directions);
            for (int i = dirs.Count - 1; i >= 0; i--)
            {
                Vector2Int next = room + dirs[i];
                if (!IsInBounds(next) || _grid[next.x, next.y])
                    dirs.RemoveAt(i);
            }

            if (dirs.Count == 0) continue;

            Vector2Int branchDir = dirs[Random.Range(0, dirs.Count)];
            int depth = Random.Range(1, maxBranchDepth + 1);

            Vector2Int bp = room;
            for (int d = 0; d < depth; d++)
            {
                Vector2Int next = bp + branchDir;
                if (!IsInBounds(next) || _grid[next.x, next.y]) break;
                _grid[next.x, next.y] = true;
                bp = next;
                branchesAdded++;
            }
        }
    }

    private int CountRooms()
    {
        int count = 0;
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
                if (_grid[x, y]) count++;
        return count;
    }

    private void FillToMin()
    {
        int maxIterations = gridWidth * gridHeight;
        for (int iter = 0; iter < maxIterations; iter++)
        {
            if (CountRooms() >= minRoomCount) break;

            var candidates = new List<Vector2Int>();
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (!_grid[x, y]) continue;
                    foreach (var dir in Directions)
                    {
                        Vector2Int next = new Vector2Int(x + dir.x, y + dir.y);
                        if (IsInBounds(next) && !_grid[next.x, next.y])
                            candidates.Add(next);
                    }
                }
            }

            if (candidates.Count == 0) break;

            var chosen = candidates[Random.Range(0, candidates.Count)];
            _grid[chosen.x, chosen.y] = true;
        }
    }

    private void CreateRoomObjects()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (!_grid[x, y]) continue;

                RoomType type = RoomType.Normal;
                if (new Vector2Int(x, y) == _entrancePos) type = RoomType.Entrance;
                else if (new Vector2Int(x, y) == _exitPos) type = RoomType.Exit;

                GameObject roomGO;
                if (roomPrefab != null)
                {
                    roomGO = Instantiate(roomPrefab, _roomsContainer);
                }
                else
                {
                    roomGO = new GameObject($"Room_{x}_{y}", typeof(RectTransform), typeof(Image));
                    roomGO.transform.SetParent(_roomsContainer, false);
                }

                Room room = roomGO.GetComponent<Room>();
                if (room == null)
                    room = roomGO.AddComponent<Room>();

                room.Setup(x, y, type);

                Vector2 gridOrigin = GetGridOrigin();
                Vector2 position = new Vector2(
                    gridOrigin.x + x * spacing,
                    gridOrigin.y + y * spacing
                );

                RectTransform rt = roomGO.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(roomSize, roomSize);
                rt.anchoredPosition = position;

                if (roomGO.GetComponent<Button>() == null && roomGO.GetComponent<Image>() != null)
                {
                    var button = roomGO.AddComponent<Button>();
                    button.targetGraphic = roomGO.GetComponent<Image>();
                    button.transition = Selectable.Transition.ColorTint;
                    var colors = button.colors;
                    colors.highlightedColor = new Color(1f, 1f, 0.5f, 1f);
                    colors.selectedColor = new Color(1f, 1f, 0f, 1f);
                    button.colors = colors;

                    int cx = x, cy = y;
                    button.onClick.AddListener(() => OnRoomClick(cx, cy));
                }

                _rooms[x, y] = room;
            }
        }
    }

    private void CreateConnections()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (_rooms[x, y] == null) continue;

                if (x + 1 < gridWidth && _rooms[x + 1, y] != null)
                    CreateLine(_rooms[x, y], _rooms[x + 1, y]);

                if (y + 1 < gridHeight && _rooms[x, y + 1] != null)
                    CreateLine(_rooms[x, y], _rooms[x, y + 1]);
            }
        }
    }

    private void CreateLine(Room a, Room b)
    {
        GameObject lineGO;

        if (linePrefab != null)
        {
            lineGO = Instantiate(linePrefab, _linesContainer);
        }
        else
        {
            lineGO = new GameObject("Line", typeof(RectTransform), typeof(Image));
            lineGO.transform.SetParent(_linesContainer, false);
            var img = lineGO.GetComponent<Image>();
            img.color = Color.white;
        }

        lineGO.name = $"Line_{a.GridX}_{a.GridY}_{b.GridX}_{b.GridY}";

        var lineCtrl = lineGO.GetComponent<LineController>();
        if (lineCtrl == null)
            lineCtrl = lineGO.AddComponent<LineController>();

        RectTransform rtA = a.GetComponent<RectTransform>();
        RectTransform rtB = b.GetComponent<RectTransform>();

        float halfRoom = roomSize * 0.5f;
        Vector2 dir = (rtB.anchoredPosition - rtA.anchoredPosition).normalized;
        Vector2 fromPos = rtA.anchoredPosition + dir * halfRoom;
        Vector2 toPos = rtB.anchoredPosition - dir * halfRoom;

        lineCtrl.Connect(fromPos, toPos);
    }

    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int pos, bool[,] visited)
    {
        var result = new List<Vector2Int>();
        foreach (var dir in Directions)
        {
            Vector2Int next = pos + dir;
            if (IsInBounds(next) && !visited[next.x, next.y])
                result.Add(next);
        }
        return result;
    }

    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
    }

    private Vector2 GetGridOrigin()
    {
        float totalWidth = (gridWidth - 1) * spacing;
        float totalHeight = (gridHeight - 1) * spacing;
        return new Vector2(-totalWidth * 0.5f, -totalHeight * 0.5f);
    }

    private void OnRoomClick(int x, int y)
    {
        RoomType type = RoomType.Normal;
        if (new Vector2Int(x, y) == _entrancePos) type = RoomType.Entrance;
        else if (new Vector2Int(x, y) == _exitPos) type = RoomType.Exit;
        Debug.Log($"[Dungeon] Room clicked: ({x}, {y}) - {type}");
    }
}
}
