using Features.CrawlerMap.Domain;
using UnityEngine;

namespace Features.CrawlerMap.Runtime
{
    public class MapRenderer : MonoBehaviour
    {
        [SerializeField] private CrawlerMapData _mapData;
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private GameObject _cellPrefab;

        public CellRenderer[,] CellRenderers { get; private set; }

        public float CellSize { get => _cellSize; }

        public void Initialize(CrawlerMapData data)
        {
            _mapData = data;
            BuildGrid();
        }

        private void Start()
        {
            if (_mapData != null)
            {
                BuildGrid();
            }
        }

        private void BuildGrid()
        {
            if (_mapData == null)
            {
                Debug.LogError("MapRenderer: _mapData is null");
                return;
            }

            ClearGrid();

            CellRenderers = new CellRenderer[_mapData.Width, _mapData.Height];

            for (int y = 0; y < _mapData.Height; y++)
            {
                for (int x = 0; x < _mapData.Width; x++)
                {
                    GameObject go;
                    if (_cellPrefab != null)
                    {
                        go = Instantiate(_cellPrefab, transform);
                    }
                    else
                    {
                        go = new GameObject($"Cell_{x}_{y}");
                        go.transform.SetParent(transform);
                        go.AddComponent<SpriteRenderer>();
                    }

                    CellRenderer cr = go.GetComponent<CellRenderer>();
                    if (cr == null)
                    {
                        cr = go.AddComponent<CellRenderer>();
                    }

                    GridCoord pos = new(x, y);
                    CellData cellData = _mapData.GetCell(pos);
                    cr.Init(pos, cellData, _cellSize);

                    CellRenderers[x, y] = cr;
                }
            }
        }

        public CellRenderer GetCellRenderer(int x, int y)
        {
            if (CellRenderers == null)
            {
                return null;
            }

            if (x < 0 || x >= CellRenderers.GetLength(0)
                      || y < 0 || y >= CellRenderers.GetLength(1))
            {
                return null;
            }

            return CellRenderers[x, y];
        }

        public CellData GetCellData(int x, int y)
        {
            return _mapData?.GetCell(x, y);
        }

        public bool IsWalkable(int x, int y)
        {
            CellData cell = GetCellData(x, y);
            if (cell == null)
            {
                return false;
            }

            return cell.ContentType != CellContentType.Wall
                   && cell.ContentType != CellContentType.Empty;
        }

        public Vector3 CellToWorld(GridCoord coord)
        {
            return new Vector3(coord.X * _cellSize, coord.Y * _cellSize, 0f);
        }

        private void ClearGrid()
        {
            if (CellRenderers != null)
            {
                for (int y = 0; y < CellRenderers.GetLength(1); y++)
                {
                    for (int x = 0; x < CellRenderers.GetLength(0); x++)
                    {
                        if (CellRenderers[x, y] != null)
                        {
                            Destroy(CellRenderers[x, y].gameObject);
                        }
                    }
                }
            }

            // Destroy any leftover children
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }
}