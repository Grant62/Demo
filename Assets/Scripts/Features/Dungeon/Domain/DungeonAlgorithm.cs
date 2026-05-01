using System;
using System.Collections.Generic;

namespace Features.Dungeon.Domain
{
    public class DungeonAlgorithm : IDungeonAlgorithm
    {
        private readonly Random _rng = new();

        private static readonly GridPosition[] Directions =
        {
            new(0, 1),
            new(0, -1),
            new(1, 0),
            new(-1, 0)
        };

        public DungeonResult Generate(DungeonSettings settings)
        {
            DungeonResult result = TryGenerate(settings);
            if (result != null) return result;

            return ForceGenerate(settings);
        }

        private DungeonResult TryGenerate(DungeonSettings settings)
        {
            for (int attempt = 0; attempt < settings.MaxRetries; attempt++)
            {
                bool[,] grid = new bool[settings.GridWidth, settings.GridHeight];

                GridPosition entrance = PickEntrance(settings);
                GridPosition exit = PickExit(settings, entrance);

                GenerateMainPath(grid, settings, entrance, exit);
                AddBranches(grid, settings, entrance, exit);

                int roomCount = CountRooms(grid);

                if (roomCount < settings.MinRoomCount)
                {
                    FillToMin(grid, settings);
                    roomCount = CountRooms(grid);
                }

                if (roomCount >= settings.MinRoomCount && roomCount <= settings.MaxRoomCount)
                {
                    return BuildResult(grid, entrance, exit);
                }
            }

            return null;
        }

        private DungeonResult ForceGenerate(DungeonSettings settings)
        {
            bool[,] grid = new bool[settings.GridWidth, settings.GridHeight];
            GridPosition entrance = PickEntrance(settings);
            GridPosition exit = PickExit(settings, entrance);

            GenerateMainPath(grid, settings, entrance, exit);
            FillToMin(grid, settings);

            return BuildResult(grid, entrance, exit);
        }

        private GridPosition PickEntrance(DungeonSettings settings)
        {
            int edge = _rng.Next(2);
            return edge == 0
                ? new GridPosition(0, _rng.Next(settings.GridHeight))
                : new GridPosition(_rng.Next(settings.GridWidth), 0);
        }

        private GridPosition PickExit(DungeonSettings settings, GridPosition entrance)
        {
            GridPosition exit;
            int attempts = 0;
            do
            {
                int edge = _rng.Next(2);
                exit = edge == 0
                    ? new GridPosition(settings.GridWidth - 1, _rng.Next(settings.GridHeight))
                    : new GridPosition(_rng.Next(settings.GridWidth), settings.GridHeight - 1);
                attempts++;
            } while (exit == entrance && attempts < 10);

            return exit;
        }

        private void GenerateMainPath(bool[,] grid, DungeonSettings settings, GridPosition entrance, GridPosition exit)
        {
            int width = settings.GridWidth;
            int height = settings.GridHeight;
            bool[,] visited = new bool[width, height];
            List<GridPosition> path = new()
                { entrance };
            visited[entrance.X, entrance.Y] = true;

            GridPosition current = entrance;
            int maxSteps = width * height * 3;

            for (int i = 0; i < maxSteps && current != exit; i++)
            {
                List<GridPosition> candidates = GetUnvisitedNeighbors(current, visited, width, height);
                if (candidates.Count == 0) break;

                float totalWeight = 0f;
                float[] weights = new float[candidates.Count];
                for (int j = 0; j < candidates.Count; j++)
                {
                    float distBefore = Dist(current, exit);
                    float distAfter = Dist(candidates[j], exit);
                    float improvement = distBefore - distAfter;
                    weights[j] = MathF.Max(0.1f, 1f + improvement * 5f);
                    totalWeight += weights[j];
                }

                float r = (float)_rng.NextDouble() * totalWeight;
                float cumulative = 0f;
                int chosen = 0;
                for (int j = 0; j < weights.Length; j++)
                {
                    cumulative += weights[j];
                    if (r <= cumulative)
                    {
                        chosen = j;
                        break;
                    }
                }

                current = candidates[chosen];
                path.Add(current);
                visited[current.X, current.Y] = true;
            }

            if (current != exit)
            {
                ForceConnectToExit(path, visited, exit, width, height);
            }

            foreach (GridPosition pos in path)
                grid[pos.X, pos.Y] = true;
        }

        private static void ForceConnectToExit(List<GridPosition> path, bool[,] visited, GridPosition exit, int width, int height)
        {
            GridPosition cursor = path[^1];

            while (cursor != exit)
            {
                int dx = Math.Clamp(exit.X - cursor.X, -1, 1);
                int dy = Math.Clamp(exit.Y - cursor.Y, -1, 1);

                if (dx != 0 && dy != 0)
                {
                    if (new Random().NextDouble() < 0.5) dy = 0;
                    else dx = 0;
                }

                int nx = cursor.X + dx;
                int ny = cursor.Y + dy;

                if (!IsInBounds(nx, ny, width, height)) break;

                if (!visited[nx, ny])
                {
                    cursor = new GridPosition(nx, ny);
                    path.Add(cursor);
                    visited[cursor.X, cursor.Y] = true;
                }
                else
                {
                    if (dx != 0 && IsInBounds(cursor.X + dx, cursor.Y, width, height) && !visited[cursor.X + dx, cursor.Y])
                        cursor = new GridPosition(cursor.X + dx, cursor.Y);
                    else if (dy != 0 && IsInBounds(cursor.X, cursor.Y + dy, width, height) && !visited[cursor.X, cursor.Y + dy])
                        cursor = new GridPosition(cursor.X, cursor.Y + dy);
                    else
                        break;
                }
            }
        }

        private void AddBranches(bool[,] grid, DungeonSettings settings, GridPosition entrance, GridPosition exit)
        {
            int width = settings.GridWidth;
            int height = settings.GridHeight;

            List<GridPosition> mainPathRooms = new();
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (grid[x, y])
                    mainPathRooms.Add(new GridPosition(x, y));

            int branchesAdded = 0;
            foreach (GridPosition room in mainPathRooms)
            {
                if (room == entrance || room == exit) continue;
                if (branchesAdded >= settings.MaxBranchCount) break;
                if ((float)_rng.NextDouble() > settings.BranchProbability) continue;

                List<GridPosition> dirs = new(Directions);
                for (int i = dirs.Count - 1; i >= 0; i--)
                {
                    GridPosition next = room + dirs[i];
                    if (!IsInBounds(next.X, next.Y, width, height) || grid[next.X, next.Y])
                        dirs.RemoveAt(i);
                }

                if (dirs.Count == 0) continue;

                GridPosition branchDir = dirs[_rng.Next(dirs.Count)];
                int depth = _rng.Next(1, settings.MaxBranchDepth + 1);

                GridPosition bp = room;
                for (int d = 0; d < depth; d++)
                {
                    GridPosition next = bp + branchDir;
                    if (!IsInBounds(next.X, next.Y, width, height) || grid[next.X, next.Y]) break;
                    grid[next.X, next.Y] = true;
                    bp = next;
                    branchesAdded++;
                }
            }
        }

        private void FillToMin(bool[,] grid, DungeonSettings settings)
        {
            int width = settings.GridWidth;
            int height = settings.GridHeight;
            int maxIterations = width * height;

            for (int iter = 0; iter < maxIterations; iter++)
            {
                if (CountRooms(grid) >= settings.MinRoomCount) break;

                List<GridPosition> candidates = new();
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (!grid[x, y]) continue;
                        foreach (GridPosition dir in Directions)
                        {
                            int nx = x + dir.X;
                            int ny = y + dir.Y;
                            if (IsInBounds(nx, ny, width, height) && !grid[nx, ny])
                                candidates.Add(new GridPosition(nx, ny));
                        }
                    }
                }

                if (candidates.Count == 0) break;

                GridPosition chosen = candidates[_rng.Next(candidates.Count)];
                grid[chosen.X, chosen.Y] = true;
            }
        }

        private static DungeonResult BuildResult(bool[,] grid, GridPosition entrance, GridPosition exit)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            List<RoomData> rooms = new();
            List<LineData> lines = new();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!grid[x, y]) continue;

                    GridPosition pos = new(x, y);
                    RoomType type = RoomType.Normal;
                    if (pos == entrance) type = RoomType.Entrance;
                    else if (pos == exit) type = RoomType.Exit;

                    rooms.Add(new RoomData(pos, type));
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!grid[x, y]) continue;

                    if (x + 1 < width && grid[x + 1, y])
                        lines.Add(new LineData(new GridPosition(x, y), new GridPosition(x + 1, y)));

                    if (y + 1 < height && grid[x, y + 1])
                        lines.Add(new LineData(new GridPosition(x, y), new GridPosition(x, y + 1)));
                }
            }

            return new DungeonResult(rooms, lines);
        }

        private static int CountRooms(bool[,] grid)
        {
            int count = 0;
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (grid[x, y])
                    count++;
            return count;
        }

        private static List<GridPosition> GetUnvisitedNeighbors(GridPosition pos, bool[,] visited, int width, int height)
        {
            List<GridPosition> result = new();
            foreach (GridPosition dir in Directions)
            {
                int nx = pos.X + dir.X;
                int ny = pos.Y + dir.Y;
                if (IsInBounds(nx, ny, width, height) && !visited[nx, ny])
                    result.Add(new GridPosition(nx, ny));
            }

            return result;
        }

        private static bool IsInBounds(int x, int y, int width, int height)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        private static float Dist(GridPosition a, GridPosition b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }
    }
}