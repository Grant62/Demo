using System;
using JKFrame;

namespace Main.Resource
{
    [Serializable]
    public class GameSaveData
    {
        public int gold = 5000;
        public int blueprints = 3;
        public int usedSpace;

        public void Save()
        {
            if (SaveSystem.GetSaveItem(0) == null)
            {
                SaveSystem.CreateSaveItem();
            }

            SaveSystem.SaveObject(this);
        }

        public static GameSaveData Load()
        {
            SaveItem saveItem = SaveSystem.GetSaveItem(0);
            if (saveItem == null)
            {
                return new GameSaveData();
            }

            return SaveSystem.LoadObject<GameSaveData>();
        }

        public static void Delete()
        {
            SaveSystem.DeleteSaveItem(0);
        }
    }
}