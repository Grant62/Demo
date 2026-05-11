using JKFrame;

namespace Features.Battle.Application
{
    public static class BattleFlowService
    {
        public static void StartPlayerTurn()
        {
            Domain.TurnSystem.StartPlayerTurn();
        }

        public static void StartEnemyTurn()
        {
            EventSystem.EventTrigger("EnemyTurnStart");
        }
    }
}
