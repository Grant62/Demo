using Features.CrawlerMap.Domain;
using UnityEngine;

namespace Features.CrawlerMap.Runtime
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CellRenderer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        public GridCoord GridPos { get; private set; }

        private static readonly Color ColorSpace = new(0.85f, 0.85f, 0.85f);
        private static readonly Color ColorWall = new(0f, 0f, 0f);
        private static readonly Color ColorEvent = new(0.30f, 0.55f, 1.00f);
        private static readonly Color ColorItem = new(0.30f, 0.90f, 0.35f);
        private static readonly Color ColorEnemy = new(0.95f, 0.30f, 0.30f);
        private static readonly Color ColorEmpty = new(0.15f, 0.15f, 0.17f);

        private void Awake()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public void Init(GridCoord pos, CellData cellData, float cellSize)
        {
            GridPos = pos;
            transform.localPosition = new Vector3(pos.X * cellSize, pos.Y * cellSize, 0f);
            ApplyCell(cellData);
        }

        public void ApplyCell(CellData cellData)
        {
            if (cellData == null)
            {
                _spriteRenderer.color = ColorEmpty;
                return;
            }

            switch (cellData.ContentType)
            {
                case CellContentType.Empty:
                    _spriteRenderer.color = ColorEmpty;
                    break;
                case CellContentType.Wall:
                    _spriteRenderer.color = ColorWall;
                    break;
                case CellContentType.Space:
                    _spriteRenderer.color = ColorSpace;
                    break;
                case CellContentType.Event:
                    _spriteRenderer.color = ColorEvent;
                    break;
                case CellContentType.Item:
                    _spriteRenderer.color = ColorItem;
                    break;
                case CellContentType.Enemy:
                    _spriteRenderer.color = ColorEnemy;
                    break;
                default:
                    _spriteRenderer.color = ColorEmpty;
                    break;
            }
        }
    }
}