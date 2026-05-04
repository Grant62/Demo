using System;
using UnityEngine;
using Random = System.Random;

namespace Features.CrawlerMap.Domain
{
    [Serializable]
    public struct GenerationSettings
    {
        public int seed;
        public int blockCount;
        public int minWidth;
        public int maxWidth;
        public int minHeight;
        public int maxHeight;
        public int margin;
        public bool fillWalls;

        public static GenerationSettings Default()
        {
            return new GenerationSettings
            {
                seed = 0,
                blockCount = 8,
                minWidth = 2,
                maxWidth = 4,
                minHeight = 2,
                maxHeight = 4,
                margin = 1,
                fillWalls = true
            };
        }
    }

    public static class MapGenerator
    {
        public static void Generate(CrawlerMapData data, GenerationSettings settings)
        {
            data.ClearAll();

            if (data.Width < 2 || data.Height < 2)
            {
                Debug.LogError("Map too small");
                return;
            }

            Random rng = settings.seed != 0
                ? new Random(settings.seed)
                : new Random();

            int maxAttempts = settings.blockCount * 50;
            int placed = 0;

            for (int attempt = 0; attempt < maxAttempts && placed < settings.blockCount; attempt++)
            {
                int w = rng.Next(settings.minWidth, settings.maxWidth + 1);
                int h = rng.Next(settings.minHeight, settings.maxHeight + 1);

                int ox = rng.Next(0, data.Width - w + 1);
                int oy = rng.Next(0, data.Height - h + 1);

                if (CanPlace(data, ox, oy, w, h, settings.margin))
                {
                    RoomBlock block = new()
                    {
                        Name = $"Gen_{placed + 1}",
                        OriginX = ox,
                        OriginY = oy,
                        Width = w,
                        Height = h
                    };
                    data.ApplyBlock(block);
                    placed++;
                }
            }

            if (settings.fillWalls)
            {
                FillWalls(data);
            }

            Debug.Log($"MapGenerator: placed {placed} blocks on {data.Width}x{data.Height}");
        }

        private static bool CanPlace(CrawlerMapData data, int ox, int oy, int w, int h, int margin)
        {
            int checkLeft = Mathf.Max(0, ox - margin);
            int checkRight = Mathf.Min(data.Width - 1, ox + w - 1 + margin);
            int checkBottom = Mathf.Max(0, oy - margin);
            int checkTop = Mathf.Min(data.Height - 1, oy + h - 1 + margin);

            for (int x = checkLeft; x <= checkRight; x++)
            {
                for (int y = checkBottom; y <= checkTop; y++)
                {
                    CellData cell = data.GetCell(x, y);
                    if (cell != null && cell.ContentType != CellContentType.Empty)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void FillWalls(CrawlerMapData data)
        {
            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    CellData cell = data.GetCell(x, y);
                    if (cell != null && cell.ContentType == CellContentType.Empty)
                    {
                        cell.ContentType = CellContentType.Wall;
                    }
                }
            }
        }
    }
}