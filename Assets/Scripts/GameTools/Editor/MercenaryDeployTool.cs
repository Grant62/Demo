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
    public class MercenaryDeployTool : EditorWindow
    {
        private const string ConfigPath = "Assets/GameResource/Config/MercenaryDeployConfig.asset";

        private static readonly Dictionary<int, string> OccuGroupNames = new()
        {
            { 1, "枪兵系" },
            { 2, "弓兵系" },
            { 3, "牧师系" },
            { 4, "剑客系" },
            { 5, "渴血系" },
            { 6, "刺客系" },
            { 7, "法师系" },
            { 8, "盗贼系" }
        };

        private Dictionary<int, List<OccupationInfo>> _grouped;
        private List<int> _orderedOccuIds;
        private MercenaryDeployConfig _config;
        private Vector2 _scrollPos;

        private void OnEnable()
        {
            LoadOccupationData();
            _config = AssetDatabase.LoadAssetAtPath<MercenaryDeployConfig>(ConfigPath);
            if (_config == null)
            {
                _config = CreateInstance<MercenaryDeployConfig>();
                _config.occupationIds = new List<int>();
            }
        }

        private void LoadOccupationData()
        {
            OccupationInfoContainer container = BinaryDataMgr.Ins.GetTable<OccupationInfoContainer>();
            if (container == null) return;

            _grouped = new Dictionary<int, List<OccupationInfo>>();
            _orderedOccuIds = new List<int>();

            foreach (OccupationInfo info in container.DataDic.Values)
            {
                if (!_grouped.ContainsKey(info.OccuId))
                {
                    _grouped[info.OccuId] = new List<OccupationInfo>();
                    _orderedOccuIds.Add(info.OccuId);
                }

                _grouped[info.OccuId].Add(info);
            }
        }

        private string GetGroupName(int occuId)
        {
            return OccuGroupNames.TryGetValue(occuId, out string name) ? name : $"职业{occuId}";
        }

        private void OnGUI()
        {
            if (_grouped == null)
            {
                EditorGUILayout.HelpBox("请先构建二进制配置表", MessageType.Warning);
                return;
            }

            int selected = _config.occupationIds.Count;
            EditorGUILayout.LabelField($"已选 {selected}/5", EditorStyles.boldLabel);

            if (selected > 5)
            {
                EditorGUILayout.HelpBox("最多选择 5 个！", MessageType.Error);
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            // 表头
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("职业名称", EditorStyles.boldLabel, GUILayout.Width(80));
            EditorGUILayout.LabelField("Id", EditorStyles.boldLabel, GUILayout.Width(30));
            EditorGUILayout.LabelField("能量耗费", new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, padding = new RectOffset(0, 0, 0, 0) }, GUILayout.Width(60));
            GUILayout.Space(10);
            EditorGUILayout.LabelField("效果描述", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            foreach (int occuId in _orderedOccuIds)
            {
                List<OccupationInfo> list = _grouped[occuId];
                EditorGUILayout.LabelField(GetGroupName(occuId), EditorStyles.boldLabel);

                foreach (OccupationInfo info in list)
                {
                    bool isSelected = _config.occupationIds.Contains(info.Id);
                    bool wasSelected = isSelected;

                    EditorGUILayout.BeginHorizontal();
                    isSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));

                    if (isSelected != wasSelected)
                    {
                        if (isSelected)
                        {
                            if (_config.occupationIds.Count >= 5)
                            {
                                Debug.LogWarning("已达上限 5 个！");
                                isSelected = false;
                            }
                            else
                            {
                                _config.occupationIds.Add(info.Id);
                            }
                        }
                        else
                        {
                            _config.occupationIds.Remove(info.Id);
                        }
                    }

                    EditorGUILayout.LabelField(info.Name, GUILayout.Width(80));
                    EditorGUILayout.LabelField(info.Id.ToString(), GUILayout.Width(30));
                    string pointStr = info.Point == -1 ? "x" : info.Point.ToString();
                    EditorGUILayout.LabelField(pointStr, new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter, padding = new RectOffset(0, 10, 0, 0) }, GUILayout.Width(60));
                    EditorGUILayout.LabelField(info.Desc);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space(4);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("保存配置", GUILayout.Height(40)))
            {
                SaveConfig();
            }

            if (GUILayout.Button("清空选择", GUILayout.Height(24)))
            {
                _config.occupationIds.Clear();
            }
        }

        private void SaveConfig()
        {
            MercenaryDeployConfig existing = AssetDatabase.LoadAssetAtPath<MercenaryDeployConfig>(ConfigPath);
            if (existing != null)
            {
                existing.occupationIds = new List<int>(_config.occupationIds);
                EditorUtility.SetDirty(existing);
                AssetDatabase.SaveAssets();
            }
            else
            {
                string dir = Path.GetDirectoryName(ConfigPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                AssetDatabase.CreateAsset(_config, ConfigPath);
            }

            SetAddressable(ConfigPath, "MercenaryDeployConfig");

            AssetDatabase.Refresh();
            Debug.Log("雇佣兵配置已保存");
        }

        private static void SetAddressable(string assetPath, string address)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                settings = AddressableAssetSettings.Create(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder, AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true,
                    true);
            }

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            AddressableAssetEntry entry = settings.FindAssetEntry(guid);
            if (entry != null)
            {
                entry.address = address;
            }
            else
            {
                AddressableAssetGroup group = settings.FindGroup("Config");
                if (group == null)
                    group = settings.CreateGroup("Config", false, false, true, null);
                entry = settings.CreateOrMoveEntry(guid, group);
                entry.address = address;
            }

            EditorUtility.SetDirty(settings);
        }
    }
}