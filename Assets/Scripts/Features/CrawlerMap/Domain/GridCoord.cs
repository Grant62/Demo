using System;

namespace Features.CrawlerMap.Domain
{
    [Serializable]
    public readonly struct GridCoord : IEquatable<GridCoord>
    {
        public readonly int X;
        public readonly int Y;

        public GridCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public GridCoord Up { get => new(X, Y + 1); }

        public GridCoord Down { get => new(X, Y - 1); }

        public GridCoord Left { get => new(X - 1, Y); }

        public GridCoord Right { get => new(X + 1, Y); }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public bool Equals(GridCoord other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is GridCoord other && Equals(other);
        }

        public override int GetHashCode()
        {
            return X << 16 ^ Y;
        }

        public static bool operator ==(GridCoord a, GridCoord b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(GridCoord a, GridCoord b)
        {
            return !a.Equals(b);
        }

        public static GridCoord operator +(GridCoord a, GridCoord b)
        {
            return new GridCoord(a.X + b.X, a.Y + b.Y);
        }
    }
}