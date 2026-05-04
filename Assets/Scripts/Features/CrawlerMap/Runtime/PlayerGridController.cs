using Features.CrawlerMap.Domain;
using UnityEngine;

namespace Features.CrawlerMap.Runtime
{
    public class PlayerGridController : MonoBehaviour
    {
        [SerializeField] private MapRenderer _mapRenderer;
        [SerializeField] private float _moveSpeed = 8f;

        private GridCoord _gridPos;
        private Vector3 _targetPos;
        private bool _isMoving;

        private void Start()
        {
            if (_mapRenderer == null)
            {
                _mapRenderer = FindFirstObjectByType<MapRenderer>();
            }

            if (_mapRenderer != null)
            {
                _gridPos = new GridCoord(0, 0);
                _targetPos = _mapRenderer.CellToWorld(_gridPos);
                transform.position = _targetPos;
            }
        }

        private void Update()
        {
            if (_isMoving)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, _targetPos, _moveSpeed * Time.deltaTime
                );

                if (Vector3.Distance(transform.position, _targetPos) < 0.01f)
                {
                    transform.position = _targetPos;
                    _isMoving = false;
                }

                return;
            }

            GridCoord dir = GetInputDirection();
            if (dir.X != 0 || dir.Y != 0)
            {
                TryMove(dir);
            }
        }

        private static GridCoord GetInputDirection()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                return new GridCoord(0, 1);
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                return new GridCoord(0, -1);
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                return new GridCoord(-1, 0);
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                return new GridCoord(1, 0);
            }

            return new GridCoord(0, 0);
        }

        private void TryMove(GridCoord direction)
        {
            GridCoord target = _gridPos + direction;

            if (_mapRenderer != null && _mapRenderer.IsWalkable(target.X, target.Y))
            {
                _gridPos = target;
                _targetPos = _mapRenderer.CellToWorld(_gridPos);
                _isMoving = true;
            }
        }

        public void TeleportTo(GridCoord pos)
        {
            if (_mapRenderer != null && _mapRenderer.IsWalkable(pos.X, pos.Y))
            {
                _gridPos = pos;
                _targetPos = _mapRenderer.CellToWorld(_gridPos);
                transform.position = _targetPos;
                _isMoving = false;
            }
        }
    }
}