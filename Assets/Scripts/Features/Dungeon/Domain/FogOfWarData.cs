namespace Features.Dungeon.Domain
{
    public class FogOfWarData
    {
        private readonly bool[,] _revealed;
        private readonly int _originX;
        private readonly int _originY;

        public int Width { get; }
        public int Height { get; }

        public FogOfWarData(int width, int height, int originX, int originY)
        {
            Width = width;
            Height = height;
            _originX = originX;
            _originY = originY;
            _revealed = new bool[width, height];
        }

        public void Reveal(int x, int y)
        {
            int idxX = x - _originX;
            int idxY = y - _originY;

            if (idxX < 0 || idxX >= Width || idxY < 0 || idxY >= Height)
                return;

            _revealed[idxX, idxY] = true;
        }

        public bool IsRevealed(int x, int y)
        {
            int idxX = x - _originX;
            int idxY = y - _originY;

            if (idxX < 0 || idxX >= Width || idxY < 0 || idxY >= Height)
                return false;

            return _revealed[idxX, idxY];
        }
    }
}
