using System.Collections.Generic;
using UnityEngine;

namespace Features.Town.Domain
{
    public static class RecruitService
    {
        private static Dictionary<UnlockSource, List<RecruitEntry>> _pools;
        private static bool _initialized;

        private static void EnsurePools()
        {
            if (_initialized)
            {
                return;
            }

            _pools = new Dictionary<UnlockSource, List<RecruitEntry>>();

            _pools[UnlockSource.步兵营] = new List<RecruitEntry>
            {
                new(
                    "枪兵",
                    "攻击面前所有怪物，造成3点伤害",
                    1,
                    RecruitCategory.兵种,
                    UnlockSource.步兵营
                )
            };

            _pools[UnlockSource.哨兵所] = new List<RecruitEntry>
            {
                new(
                    "人类射手",
                    "攻击目标单体，造成4点伤害，该目标每受到1次伤害，额外+2伤害",
                    1,
                    RecruitCategory.兵种,
                    UnlockSource.哨兵所
                )
            };

            _pools[UnlockSource.教堂] = new List<RecruitEntry>();
            _pools[UnlockSource.盗贼工会] = new List<RecruitEntry>();
            _pools[UnlockSource.狂战士营地] = new List<RecruitEntry>();
            _pools[UnlockSource.修道院] = new List<RecruitEntry>();
            _pools[UnlockSource.魔法师协会] = new List<RecruitEntry>();

            _initialized = true;
        }

        public static List<RecruitEntry> GetPool(UnlockSource source)
        {
            EnsurePools();
            if (_pools.TryGetValue(source, out List<RecruitEntry> pool))
            {
                return pool;
            }

            return new List<RecruitEntry>();
        }

        public static RecruitEntry DrawFrom(UnlockSource source)
        {
            EnsurePools();
            if (!_pools.TryGetValue(source, out List<RecruitEntry> pool) || pool.Count == 0)
            {
                Debug.LogWarning($"招募池 {source} 为空");
                return null;
            }

            return pool[Random.Range(0, pool.Count)];
        }
    }
}