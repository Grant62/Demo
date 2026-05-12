namespace Features.Action.Domain
{
    public class EnemyTarget
    {
        public int CurrentHP;
        public int MaxHP;
        public bool HasSlow;
        public int HitCountThisTurn;

        public bool IsAlive { get => CurrentHP > 0; }
    }
}