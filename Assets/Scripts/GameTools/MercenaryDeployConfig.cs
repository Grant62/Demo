using System.Collections.Generic;
using UnityEngine;

namespace GameTools
{
    [CreateAssetMenu(fileName = "MercenaryDeployConfig", menuName = "游戏工具/雇佣兵部署配置")]
    public class MercenaryDeployConfig : ScriptableObject
    {
        public List<int> occupationIds = new();
    }
}