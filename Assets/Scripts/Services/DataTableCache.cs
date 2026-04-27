namespace Services
{
    /// <summary>
    ///     数据表缓存中心
    ///     统一管理所有 Excel 配置表的加载和缓存，避免重复调用 BinaryDataMgr
    /// </summary>
    public static class DataTableCache
    {
        #region 配置表缓存

        // public static Dictionary<int, MonsterConfig> MonsterConfigDic { get; private set; }
        #endregion

        private static bool _isInitialized;

        /// <summary>
        ///     初始化所有配置表缓存
        ///     应在游戏启动时调用一次
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized) return;

            // MonsterConfigDic = BinaryDataMgr.Ins.GetTable<MonsterConfigContainer>().DataDic;

            _isInitialized = true;
        }

        // public static bool TryGetMonsterConfig(int monsterId, out MonsterConfig monsterConfig)
        // {
        //     return MonsterConfigDic.TryGetValue(monsterId, out monsterConfig);
        // }
    }
}