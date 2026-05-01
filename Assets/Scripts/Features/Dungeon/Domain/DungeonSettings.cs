using System;

namespace Features.Dungeon.Domain
{
    [Serializable]
    public class DungeonSettings
    {
        public int GridWidth = 7;
        public int GridHeight = 7;
        public float RoomSize = 60f;
        public float Spacing = 120f;
        public int MinRoomCount = 6;
        public int MaxRoomCount = 20;
        public float BranchProbability = 0.3f;
        public int MaxBranchDepth = 2;
        public int MaxBranchCount = 4;
        public int MaxRetries = 20;
    }
}