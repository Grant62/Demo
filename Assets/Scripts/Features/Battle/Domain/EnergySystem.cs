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

        private static void Refill()
        {
            CurrentEnergy = MaxEnergy;
            EventSystem.EventTrigger("EnergyChanged");
        }
    }
}
