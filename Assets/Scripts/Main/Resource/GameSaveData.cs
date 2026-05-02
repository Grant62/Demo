using JKFrame;

namespace Main.Resource
{
    [System.Serializable]
    public class GameSaveData
    {
        public int gold = 5000;
        public int blueprints = 3;
        public int usedSpace;

        public void Save()
        {
            SaveSystem.SaveObject(this);
        }

        public static GameSaveData Load()
        {
            return SaveSystem.LoadObject<GameSaveData>();
        }

        public static void Delete()
        {
            SaveSystem.DeleteSaveItem(0);
        }
    }
}
