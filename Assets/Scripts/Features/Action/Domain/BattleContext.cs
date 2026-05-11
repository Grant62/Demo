using System.Collections.Generic;

namespace Features.Action.Domain
{
    public class BattleContext
    {
        public int CurrentTurn;
        public List<EnemyTarget> AllEnemies;
    }
}
