using JKFrame;

namespace Features.Battle.Domain
{
    public static class TurnSystem
    {
        public static int TurnCount { get; private set; }

        public static void StartPlayerTurn()
        {
            TurnCount++;
            EventSystem.EventTrigger("PlayerTurnStart");
        }

        public static void StartEnemyTurn()
        {
            EventSystem.EventTrigger("EnemyTurnStart");
        }
    }
}
