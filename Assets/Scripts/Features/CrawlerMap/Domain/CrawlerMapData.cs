using System.Collections.Generic;
using UnityEngine;

namespace Features.CrawlerMap.Domain
{
    [CreateAssetMenu(menuName = "Crawler Map/Map Data", fileName = "NewCrawlerMap")]
    public class CrawlerMapData : ScriptableObject
    {
        public int Width = 25;
        public int Height = 25;
        public List<CellData> Cells = new();
        public List<RoomBlock> Blocks = new();

        private void OnEnable()
        {
            EnsureCapacity();
        }

        public void Initialize(int width, int height)
        {
            Width = width;
            Height = height;
            Cells.Clear();
            Blocks.Clear();
            int count = width * height;
            for (int i = 0; i < count; i++)
            {
                Cells.Add(new CellData());
            }
        }

        private void EnsureCapacity()
        {
            int expected = Width * Height;
            while (Cells.Count < expected)
            {
                Cells.Add(new CellData());
            }
        }

        public CellData GetCell(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return null;
            }

            EnsureCapacity();
            return Cells[y * Width + x];
        }

        public CellData GetCell(GridCoord coord)
        {
            return GetCell(coord.X, coord.Y);
        }

        public void SetCell(int x, int y, CellData data)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return;
            }

            EnsureCapacity();
            Cells[y * Width + x] = data;
        }

        public void SetCell(GridCoord coord, CellData data)
        {
            SetCell(coord.X, coord.Y, data);
        }

        public void ApplyBlock(RoomBlock block)
        {
            for (int x = block.OriginX; x < block.OriginX + block.Width; x++)
            {
                for (int y = block.OriginY; y < block.OriginY + block.Height; y++)
                {
                    CellData cell = GetCell(x, y);
                    if (cell != null)
                    {
                        cell.ContentType = CellContentType.Space;
                        cell.OverlayType = CellContentType.Empty;
                    }
                }
            }
            Blocks.Add(block);
        }

        public void EraseBlock(int blockIndex)
        {
            if (blockIndex < 0 || blockIndex >= Blocks.Count)
            {
                return;
            }

            RoomBlock block = Blocks[blockIndex];
            for (int x = block.OriginX; x < block.OriginX + block.Width; x++)
            {
                for (int y = block.OriginY; y < block.OriginY + block.Height; y++)
                {
                    CellData cell = GetCell(x, y);
                    if (cell != null)
                    {
                        cell.ContentType = CellContentType.Empty;
                    }
                }
            }

            Blocks.RemoveAt(blockIndex);
        }

        public void ClearAll()
        {
            int count = Width * Height;
            Cells.Clear();
            for (int i = 0; i < count; i++)
            {
                Cells.Add(new CellData());
            }
            Blocks.Clear();
        }
    }
}