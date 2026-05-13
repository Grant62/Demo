using Features.Dungeon.Domain;

namespace Features.Dungeon.Application
{
    public static class DungeonService
    {
        public static bool IsGameOver { get; private set; }

        public static void OnPlayerReachedExport()
        {
            if (IsGameOver)
                return;

            IsGameOver = true;
            UnityEngine.Debug.Log("游戏结束");
        }

        public static void ResetGame()
        {
            IsGameOver = false;
        }
    }
}
