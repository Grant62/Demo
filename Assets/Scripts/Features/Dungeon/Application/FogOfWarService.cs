using System.Collections.Generic;
using Features.Dungeon.Domain;
using UnityEngine;

namespace Features.Dungeon.Application
{
    public static class FogOfWarService
    {
        private static FogOfWarData _data;
        private static List<Vector2Int> _offsets;

        public static int VisionRange { get; private set; }

        public static void Initialize(int width, int height)
        {
            int originX = -width / 2;
            int originY = -height / 2;
            _data = new FogOfWarData(width, height, originX, originY);
        }

        public static void SetVisionRange(int range)
        {
            VisionRange = range;

            _offsets = new List<Vector2Int>();
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) <= range)
                    {
                        _offsets.Add(new Vector2Int(dx, dy));
                    }
                }
            }
        }

        public static void RevealCells(int gridX, int gridY)
        {
            if (_data == null)
                return;

            foreach (Vector2Int offset in _offsets)
            {
                _data.Reveal(gridX + offset.x, gridY + offset.y);
            }
        }

        public static bool IsRevealed(int gridX, int gridY)
        {
            return _data != null && _data.IsRevealed(gridX, gridY);
        }
    }
}
