using System.Collections.Generic;
using System.IO;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace GameTools.Editor
{
    public class CombatConfigTool : EditorWindow
    {
        private int _tab;

        // 雇佣兵
        private Dictionary<int, List<OccupationInfo>> _occuGrouped;
        private List<int> _orderedOccuIds;
        private MercenaryDeployConfig _mercConfig;
        private static readonly Dictionary<int, string> OccuGroupNames = new()
        {
            { 1, "枪兵系" }, { 2, "弓兵系" }, { 3, "牧师系" },
            { 4, "剑客系" }, { 5, "渴血系" }, { 6, "刺客系" },
            { 7, "法师系" }, { 8, "盗贼系" }
        };

        // 道具
        private List<PropInfo> _allProps;
        private PropDeployConfig _propConfig;

        // 法术
        private List<SpellInfo> _allSpells;
        private SpellDeployConfig _spellConfig;

        // 敌人
        private List<EnemyInfo> _allEnemies;
        private EnemyDeployConfig _enemyConfig;

        private Vector2 _scrollPos;

        [MenuItem("游戏工具/战斗配置")]
        private static void Open()
        {
            GetWindow<CombatConfigTool>("战斗配置");
        }

        private void OnEnable()
        {
            LoadMercData();
            LoadPropData();
            LoadSpellData();
            LoadEnemyData();
        }

        private void LoadMercData()
        {
            OccupationInfoContainer c = BinaryDataMgr.Ins.GetTable<OccupationInfoContainer>();
            if (c == null) return;
            _occuGrouped = new Dictionary<int, List<OccupationInfo>>();
            _orderedOccuIds = new List<int>();
            foreach (OccupationInfo info in c.DataDic.Values)
            {
                if (!_occuGrouped.ContainsKey(info.OccuId))
                {
                    _occuGrouped[info.OccuId] = new List<OccupationInfo>();
                    _orderedOccuIds.Add(info.OccuId);
                }

                _occuGrouped[info.OccuId].Add(info);
            }

            _mercConfig = AssetDatabase.LoadAssetAtPath<MercenaryDeployConfig>("Assets/GameResource/Config/MercenaryDeployConfig.asset");
            if (_mercConfig == null)
            {
                _mercConfig = CreateInstance<MercenaryDeployConfig>();
                _mercConfig.occupationIds = new List<int>();
            }
        }

        private void LoadPropData()
        {
            PropInfoContainer c = BinaryDataMgr.Ins.GetTable<PropInfoContainer>();
            if (c == null) return;
            _allProps = new List<PropInfo>(c.DataDic.Values);
            _propConfig = AssetDatabase.LoadAssetAtPath<PropDeployConfig>("Assets/GameResource/Config/PropDeployConfig.asset");
            if (_propConfig == null)
            {
                _propConfig = CreateInstance<PropDeployConfig>();
                _propConfig.slots = new List<PropSlotConfig>();
            }
        }

        private void LoadSpellData()
        {
            SpellInfoContainer c = BinaryDataMgr.Ins.GetTable<SpellInfoContainer>();
            if (c == null) return;
            _allSpells = new List<SpellInfo>(c.DataDic.Values);
            _spellConfig = AssetDatabase.LoadAssetAtPath<SpellDeployConfig>("Assets/GameResource/Config/SpellDeployConfig.asset");
            if (_spellConfig == null)
            {
                _spellConfig = CreateInstance<SpellDeployConfig>();
                _spellConfig.spellIds = new List<int>();
            }
        }

        private void LoadEnemyData()
        {
            EnemyInfoContainer c = BinaryDataMgr.Ins.GetTable<EnemyInfoContainer>();
            if (c == null) return;
            _allEnemies = new List<EnemyInfo>(c.DataDic.Values);
            _enemyConfig = AssetDatabase.LoadAssetAtPath<EnemyDeployConfig>("Assets/GameResource/Config/EnemyDeployConfig.asset");
            if (_enemyConfig == null)
            {
                _enemyConfig = CreateInstance<EnemyDeployConfig>();
                _enemyConfig.enemyIds = new List<int>();
            }
        }

        private void OnGUI()
        {
            _tab = GUILayout.Toolbar(_tab, new[] { "雇佣兵", "道具", "法术", "敌人" });
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            switch (_tab)
            {
                case 0: DrawMercTab(); break;
                case 1: DrawPropTab(); break;
                case 2: DrawSpellTab(); break;
                case 3: DrawEnemyTab(); break;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(10);

            if (GUILayout.Button("保存配置", GUILayout.Height(40)))
            {
                SaveMercConfig();
                SavePropConfig();
                SaveSpellConfig();
                SaveEnemyConfig();
                AssetDatabase.Refresh();
                Debug.Log("战斗配置已全部保存");
            }
        }

        #region 雇佣兵
        private void DrawMercTab()
        {
            if (_occuGrouped == null)
            {
                EditorGUILayout.HelpBox("请先构建二进制配置表", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField($"已选 {_mercConfig.occupationIds.Count}/5", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("职业名称", EditorStyles.boldLabel, GUILayout.Width(84));
            EditorGUILayout.LabelField("Id", EditorStyles.boldLabel, GUILayout.Width(31));
            EditorGUILayout.LabelField("能量耗费", new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, padding = new RectOffset(0, 0, 0, 0) }, GUILayout.Width(60));
            GUILayout.Space(10);
            EditorGUILayout.LabelField("效果描述", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            foreach (int occuId in _orderedOccuIds)
            {
                List<OccupationInfo> list = _occuGrouped[occuId];
                EditorGUILayout.LabelField(GetGroupName(occuId), EditorStyles.boldLabel);
                foreach (OccupationInfo info in list)
                {
                    bool isSelected = _mercConfig.occupationIds.Contains(info.Id);
                    bool wasSelected = isSelected;
                    EditorGUILayout.BeginHorizontal();
                    isSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));
                    if (isSelected != wasSelected)
                    {
                        if (isSelected && _mercConfig.occupationIds.Count >= 5)
                        {
                            Debug.LogWarning("已达上限 5 个！");
                            isSelected = false;
                        }
                        else if (isSelected) _mercConfig.occupationIds.Add(info.Id);
                        else _mercConfig.occupationIds.Remove(info.Id);
                    }

                    EditorGUILayout.LabelField(info.Name, GUILayout.Width(80));
                    EditorGUILayout.LabelField(info.Id.ToString(), GUILayout.Width(30));
                    string pointStr = info.Point == -1 ? "x" : info.Point.ToString();
                    EditorGUILayout.LabelField(pointStr, new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter, padding = new RectOffset(0, 0, 0, 0) }, GUILayout.Width(60));
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField(info.Desc);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space(4);
            }
        }

        private string GetGroupName(int occuId)
        {
            return OccuGroupNames.TryGetValue(occuId, out string n) ? n : $"职业{occuId}";
        }
        #endregion

        #region 道具
        private void DrawPropTab()
        {
            if (_allProps == null)
            {
                EditorGUILayout.HelpBox("请先构建二进制配置表", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField($"已选 {_propConfig.slots.Count}/6", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("道具名称", EditorStyles.boldLabel, GUILayout.Width(78));
            EditorGUILayout.LabelField("单价", EditorStyles.boldLabel, GUILayout.Width(60));
            EditorGUILayout.LabelField("携带上限", EditorStyles.boldLabel, GUILayout.Width(60));
            GUILayout.Space(18);
            EditorGUILayout.LabelField("携带数量", EditorStyles.boldLabel, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            foreach (PropInfo prop in _allProps)
            {
                string sid = prop.Id.ToString();
                int idx = _propConfig.slots.FindIndex(s => s.itemId == sid);
                bool isSelected = idx >= 0;
                EditorGUILayout.BeginHorizontal();
                bool wasSelected = isSelected;
                isSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));
                if (isSelected != wasSelected)
                {
                    if (isSelected)
                    {
                        if (_propConfig.slots.Count >= 6)
                        {
                            Debug.LogWarning("已达上限 6 个！");
                            isSelected = false;
                        }
                        else _propConfig.slots.Add(new PropSlotConfig { itemId = sid, quantity = prop.StackLimit });
                    }
                    else _propConfig.slots.RemoveAt(idx);
                }

                EditorGUILayout.LabelField(prop.Name, GUILayout.Width(70));
                EditorGUILayout.LabelField(prop.Price.ToString(), GUILayout.Width(60));
                GUILayout.Space(20);
                EditorGUILayout.LabelField(prop.StackLimit.ToString(), GUILayout.Width(60));
                if (isSelected)
                {
                    PropSlotConfig slot = _propConfig.slots.Find(s => s.itemId == sid);
                    int newVal = EditorGUILayout.IntField(slot.quantity, GUILayout.Width(60));
                    if (newVal <= 0)
                    {
                        _propConfig.slots.RemoveAll(s => s.itemId == sid);
                        isSelected = false;
                    }
                    else if (newVal > prop.StackLimit) slot.quantity = prop.StackLimit;
                    else slot.quantity = newVal;
                }
                else EditorGUILayout.LabelField("", GUILayout.Width(60));

                EditorGUILayout.EndHorizontal();
            }
        }
        #endregion

        #region 法术
        private void DrawSpellTab()
        {
            if (_allSpells == null)
            {
                EditorGUILayout.HelpBox("请先构建二进制配置表", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField($"已选 {_spellConfig.spellIds.Count}", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("法术名称", EditorStyles.boldLabel, GUILayout.Width(80));
            EditorGUILayout.LabelField("MP消耗", EditorStyles.boldLabel, GUILayout.Width(60));
            GUILayout.Space(27);
            EditorGUILayout.LabelField("效果描述", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            foreach (SpellInfo spell in _allSpells)
            {
                bool isSelected = _spellConfig.spellIds.Contains(spell.Id);
                EditorGUILayout.BeginHorizontal();
                bool wasSelected = isSelected;
                isSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));
                if (isSelected != wasSelected)
                {
                    if (isSelected) _spellConfig.spellIds.Add(spell.Id);
                    else _spellConfig.spellIds.Remove(spell.Id);
                }

                EditorGUILayout.LabelField(spell.Name, GUILayout.Width(90));
                EditorGUILayout.LabelField(spell.Cost.ToString(), GUILayout.Width(60));
                GUILayout.Space(10);
                EditorGUILayout.LabelField(spell.Desc);
                EditorGUILayout.EndHorizontal();
            }
        }
        #endregion

        #region 敌人
        private void DrawEnemyTab()
        {
            if (_allEnemies == null)
            {
                EditorGUILayout.HelpBox("请先构建二进制配置表", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField($"已选 {_enemyConfig.enemyIds.Count}/4", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("敌人名称", EditorStyles.boldLabel, GUILayout.Width(80));
            GUILayout.Space(6);
            EditorGUILayout.LabelField("类型", EditorStyles.boldLabel, GUILayout.Width(60));
            EditorGUILayout.LabelField("攻击力", EditorStyles.boldLabel, GUILayout.Width(60));
            EditorGUILayout.LabelField("生命值", EditorStyles.boldLabel, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            foreach (EnemyInfo enemy in _allEnemies)
            {
                bool isSelected = _enemyConfig.enemyIds.Contains(enemy.Id);
                EditorGUILayout.BeginHorizontal();
                bool wasSelected = isSelected;
                isSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));
                if (isSelected != wasSelected)
                {
                    if (isSelected)
                    {
                        if (_enemyConfig.enemyIds.Count >= 4)
                        {
                            Debug.LogWarning("最多 4 个敌人！");
                            isSelected = false;
                        }
                        else _enemyConfig.enemyIds.Add(enemy.Id);
                    }
                    else _enemyConfig.enemyIds.Remove(enemy.Id);
                }

                EditorGUILayout.LabelField(enemy.Name, GUILayout.Width(80));
                EditorGUILayout.LabelField(enemy.Type, GUILayout.Width(60));
                EditorGUILayout.LabelField(enemy.InitATK.ToString(), GUILayout.Width(60));
                EditorGUILayout.LabelField(enemy.InitHP.ToString(), GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();
            }
        }
        #endregion

        #region 保存
        private void SaveMercConfig()
        {
            SaveAsset("Assets/GameResource/Config/MercenaryDeployConfig.asset", _mercConfig);
        }

        private void SavePropConfig()
        {
            SaveAsset("Assets/GameResource/Config/PropDeployConfig.asset", _propConfig);
        }

        private void SaveSpellConfig()
        {
            SaveAsset("Assets/GameResource/Config/SpellDeployConfig.asset", _spellConfig);
        }

        private void SaveEnemyConfig()
        {
            SaveAsset("Assets/GameResource/Config/EnemyDeployConfig.asset", _enemyConfig);
        }

        private void SaveAsset<T>(string path, T data) where T : ScriptableObject
        {
            T existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
            {
                EditorUtility.CopySerialized(data, existing);
                EditorUtility.SetDirty(existing);
            }
            else
            {
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                AssetDatabase.CreateAsset(data, path);
            }

            AssetDatabase.SaveAssets();
            SetAddressable(path, Path.GetFileNameWithoutExtension(path));
        }

        private static void SetAddressable(string assetPath, string address)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                settings = AddressableAssetSettings.Create(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder, AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true,
                    true);
                AddressableAssetSettingsDefaultObject.Settings = settings;
            }

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            AddressableAssetEntry entry = settings.FindAssetEntry(guid);
            if (entry != null) entry.address = address;
            else
            {
                AddressableAssetGroup group = settings.FindGroup("Config");
                if (group == null) group = settings.CreateGroup("Config", false, false, true, null);
                entry = settings.CreateOrMoveEntry(guid, group);
                entry.address = address;
            }

            EditorUtility.SetDirty(settings);
        }
        #endregion
    }
}