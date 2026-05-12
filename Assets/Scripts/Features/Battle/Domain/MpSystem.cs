using JKFrame;

namespace Features.Battle.Domain
{
    public static class MpSystem
    {
        public static int CurrentMP { get; private set; }
        public static int MaxMP { get; private set; } = 60;

        public static void Initialize(int maxMp = 60)
        {
            MaxMP = maxMp;
            CurrentMP = MaxMP;
            EventSystem.EventTrigger("MPChanged");
        }

        public static bool TrySpend(int amount)
        {
            if (CurrentMP < amount) return false;
            CurrentMP -= amount;
            EventSystem.EventTrigger("MPChanged");
            return true;
        }
    }
}