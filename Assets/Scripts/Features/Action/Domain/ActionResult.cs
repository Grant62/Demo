namespace Features.Action.Domain
{
    public class ActionResult
    {
        public int Damage;
        public int Healing;
        public int Lifesteal;
        public bool IsDodged;
        public bool TriggerFreeAction;
        public bool Stolen;
        public int TargetIndex;
        public bool TargetKilled;
    }
}