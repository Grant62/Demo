using JKFrame;

namespace Main.Resource
{
    public static class ResourceManager
    {
        public static int Gold { get; private set; } = 5000;

        public static bool TrySpendGold(int amount)
        {
            if (Gold < amount)
            {
                return false;
            }

            Gold -= amount;
            EventSystem.EventTrigger("GoldChanged");
            return true;
        }

        public static void AddGold(int amount)
        {
            Gold += amount;
            EventSystem.EventTrigger("GoldChanged");
        }
    }
}