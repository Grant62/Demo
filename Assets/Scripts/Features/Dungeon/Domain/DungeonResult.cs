using System.Collections.Generic;

namespace Features.Dungeon.Domain
{
    public class DungeonResult
    {
        public IReadOnlyList<RoomData> Rooms { get; }
        public IReadOnlyList<LineData> Lines { get; }

        public DungeonResult(List<RoomData> rooms, List<LineData> lines)
        {
            Rooms = rooms;
            Lines = lines;
        }
    }
}
