using System;
using System.Collections.Generic;

namespace Core.Infrastructure.Utils
{
    public class RandomSystem : PersistentSingleton<RandomSystem>
    {
        private int _parentSeed;
        private bool _isInitialized;
        private Dictionary<ModuleType, Random> _randomGenerators;

        public void Initialize()
        {
            if (_isInitialized) return;

            // int savedSeed = SaveManager.LoadRandomSeed();
            // if (savedSeed == 0)
            // {
            //     _parentSeed = GenerateParentSeed();
            //     SaveManager.SaveRandomSeed(_parentSeed);
            // }
            // else
            //     _parentSeed = savedSeed;

            _randomGenerators = new Dictionary<ModuleType, Random>();
            _isInitialized = true;
        }

        public Random GetRandomGenerator(ModuleType moduleType)
        {
            if (!_isInitialized) Initialize();

            if (!_randomGenerators.TryGetValue(moduleType, out Random random))
            {
                int seed = GetSeedForModule(moduleType);
                random = new Random(seed);
                _randomGenerators[moduleType] = random;
            }

            return random;
        }

        public int Range(int minInclusive, int maxExclusive, ModuleType moduleType)
        {
            Random random = GetRandomGenerator(moduleType);
            return random.Next(minInclusive, maxExclusive);
        }

        public float Value(ModuleType moduleType)
        {
            Random random = GetRandomGenerator(moduleType);
            return (float)random.NextDouble();
        }

        /// <summary>
        ///     基于房间位置生成确定性随机数，确保同一位置（相同种子+地图ID+层ID+节点ID）总是得到相同结果
        /// </summary>
        public int RangeForRoom(int minInclusive, int maxExclusive, int mapId, int layerId, int nodeIndex, int sequenceIndex = 0)
        {
            int seed = GenerateRoomSeed(mapId, layerId, nodeIndex, sequenceIndex);
            Random random = new(seed);
            return random.Next(minInclusive, maxExclusive);
        }

        /// <summary>
        ///     生成房间特定的种子
        /// </summary>
        private int GenerateRoomSeed(int mapId, int layerId, int nodeIndex, int sequenceIndex)
        {
            unchecked
            {
                int seed = _parentSeed;
                seed = seed * 31 + mapId;
                seed = seed * 31 + layerId;
                seed = seed * 31 + nodeIndex;
                seed = seed * 31 + sequenceIndex;
                return seed;
            }
        }

        public void SetParentSeed(int parentSeed)
        {
            _parentSeed = parentSeed;
            _isInitialized = true;

            // SaveManager.SaveRandomSeed(_parentSeed);

            _randomGenerators?.Clear();
        }

        /// <summary>
        ///     从存档重新加载随机种子，用于读档后确保随机状态一致
        /// </summary>
        public void ReloadSeedFromSave()
        {
            // int savedSeed = SaveManager.LoadRandomSeed();
            // if (savedSeed != 0)
            // {
            //     _parentSeed = savedSeed;
            //     _randomGenerators?.Clear();
            // }
        }

        private int GetSeedForModule(ModuleType moduleType)
        {
            unchecked
            {
                int hash = (int)moduleType;
                int seed = _parentSeed ^ hash * 397;
                return seed;
            }
        }

        public int GenerateParentSeed()
        {
            unchecked
            {
                long ticks = DateTime.Now.Ticks;
                int hash = Guid.NewGuid().GetHashCode();
                return (int)(ticks & 0xFFFFFFFF) ^ hash;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }
    }

    public enum ModuleType
    {
        Map,
        Combat,
        Card,
        Room,
        Utility
    }
}