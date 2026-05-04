using System;

namespace Features.CrawlerMap.Domain
{
    [Serializable]
    public class RoomBlock
    {
        public string Name = "Room";
        public int OriginX;
        public int OriginY;
        public int Width = 2;
        public int Height = 2;

        public GridCoord Origin { get => new(OriginX, OriginY); }

        public GridCoord Size { get => new(Width, Height); }

        public bool Contains(int x, int y)
        {
            return x >= OriginX && x < OriginX + Width
                                && y >= OriginY && y < OriginY + Height;
        }
    }
}