using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameTools
{
    [Serializable]
    public class PropSlotConfig
    {
        public string itemId;
        public int quantity;
    }

    public class PropDeployConfig : ScriptableObject
    {
        public List<PropSlotConfig> slots = new();
    }
}