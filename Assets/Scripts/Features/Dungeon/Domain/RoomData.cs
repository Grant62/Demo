namespace Features.Dungeon.Domain
{
    public readonly struct RoomData
    {
        public readonly GridPosition Position;
        public readonly RoomType Type;

        public RoomData(GridPosition position, RoomType type)
        {
            Position = position;
            Type = type;
        }
    }
}