using Features.Dungeon.Application;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Features.Dungeon.Infrastructure
{
    [RequireComponent(typeof(Tilemap))]
    public class FogOfWarRenderer : MonoBehaviour
    {
        [SerializeField] private int _mapWidth = 12;
        [SerializeField] private int _mapHeight = 10;
        [SerializeField] private Tile _fogTile;

        private Tilemap _fogLayer;

        private void Awake()
        {
            _fogLayer = GetComponent<Tilemap>();
        }

        private void Start()
        {
            FogOfWarService.Initialize(_mapWidth, _mapHeight);
            FillAllFog();
        }

        private void FillAllFog()
        {
            int originX = -_mapWidth / 2;
            int originY = -_mapHeight / 2;

            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapHeight; y++)
                {
                    _fogLayer.SetTile(new Vector3Int(originX + x, originY + y, 0), _fogTile);
                }
            }
        }

        public void RevealAt(Vector3 worldPos)
        {
            Vector3Int cellPos = _fogLayer.WorldToCell(worldPos);
            FogOfWarService.RevealCells(cellPos.x, cellPos.y);

            int range = FogOfWarService.VisionRange;
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) > range)
                        continue;

                    Vector3Int pos = cellPos + new Vector3Int(dx, dy, 0);
                    if (FogOfWarService.IsRevealed(pos.x, pos.y))
                    {
                        _fogLayer.SetTile(pos, null);
                    }
                }
            }
        }
    }
}
