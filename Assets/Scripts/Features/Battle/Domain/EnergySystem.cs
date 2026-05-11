using JKFrame;

namespace Features.Battle.Domain
{
    public static class EnergySystem
    {
        public static int CurrentEnergy { get; private set; }
        public static int MaxEnergy { get; private set; } = 3;

        public static void Initialize()
        {
            MaxEnergy = 3;
            Refill();
            EventSystem.AddEventListener("PlayerTurnStart", Refill);
        }

        public static bool TrySpend(int amount)
        {
            if (CurrentEnergy < amount)
            {
                return false;
            }

            CurrentEnergy -= amount;
            EventSystem.EventTrigger("EnergyChanged");
            return true;
        }

        /// <summary>
        /// 按雇佣兵 Point 消耗能量：正数=固定消耗，0=免费，-1=清空全部剩余能量
        /// </summary>
        public static bool TrySpendByPoint(int point)
        {
            int cost = point >= 0 ? point : CurrentEnergy;
            return TrySpend(cost);
        }

        private static void Refill()
        {
            CurrentEnergy = MaxEnergy;
            EventSystem.EventTrigger("EnergyChanged");
        }
    }
}
