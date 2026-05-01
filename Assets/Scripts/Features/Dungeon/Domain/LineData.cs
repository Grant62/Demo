namespace Features.Dungeon.Domain
{
    public readonly struct LineData
    {
        public readonly GridPosition From;
        public readonly GridPosition To;

        public LineData(GridPosition from, GridPosition to)
        {
            From = from;
            To = to;
        }
    }
}