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
    public class PropDeployTool : EditorWindow
    {
        private const string ConfigPath = "Assets/GameResource/Config/PropDeployConfig.asset";

        private List<PropInfo> _allProps;
        private PropDeployConfig _config;
        private Vector2 _scrollPos;

        private void OnEnable()
        {
            LoadProps();
            _config = AssetDatabase.LoadAssetAtPath<PropDeployConfig>(ConfigPath);
            if (_config == null)
            {
                _config = CreateInstance<PropDeployConfig>();
                _config.slots = new List<PropSlotConfig>();
            }
        }

        private void LoadProps()
        {
            PropInfoContainer container = BinaryDataMgr.Ins.GetTable<PropInfoContainer>();
            if (container == null) return;
            _allProps = new List<PropInfo>(container.DataDic.Values);
        }

        private void OnGUI()
        {
            if (_allProps == null)
            {
                EditorGUILayout.HelpBox("请先构建二进制配置表", MessageType.Warning);
                return;
            }

            int selected = _config.slots.Count;
            EditorGUILayout.LabelField($"已选 {selected}/6", EditorStyles.boldLabel);
            if (selected > 6)
                EditorGUILayout.HelpBox("最多 6 个道具槽！", MessageType.Error);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("道具名称", EditorStyles.boldLabel, GUILayout.Width(70));
            EditorGUILayout.LabelField("单价", EditorStyles.boldLabel, GUILayout.Width(60));
            EditorGUILayout.LabelField("携带上限", EditorStyles.boldLabel, GUILayout.Width(60));
            EditorGUILayout.LabelField("携带数量", EditorStyles.boldLabel, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            foreach (PropInfo prop in _allProps)
            {
                string sid = prop.Id.ToString();
                int idx = _config.slots.FindIndex(s => s.itemId == sid);
                bool isSelected = idx >= 0;

                EditorGUILayout.BeginHorizontal();
                bool wasSelected = isSelected;
                isSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));

                if (isSelected != wasSelected)
                {
                    if (isSelected)
                    {
                        if (_config.slots.Count >= 6)
                        {
                            Debug.LogWarning("已达上限 6 个！");
                            isSelected = false;
                        }
                        else
                        {
                            _config.slots.Add(new PropSlotConfig { itemId = sid, quantity = prop.StackLimit });
                        }
                    }
                    else
                    {
                        _config.slots.RemoveAt(idx);
                    }
                }

                EditorGUILayout.LabelField(prop.Name, GUILayout.Width(70));
                EditorGUILayout.LabelField(prop.Price.ToString(), GUILayout.Width(60));
                EditorGUILayout.LabelField(prop.StackLimit.ToString(), GUILayout.Width(60));

                if (isSelected)
                {
                    PropSlotConfig slot = _config.slots.Find(s => s.itemId == sid);
                    int newVal = EditorGUILayout.IntField(slot.quantity, GUILayout.Width(60));
                    if (newVal <= 0)
                    {
                        _config.slots.RemoveAll(s => s.itemId == sid);
                        isSelected = false;
                    }
                    else if (newVal > prop.StackLimit)
                    {
                        slot.quantity = prop.StackLimit;
                    }
                    else
                    {
                        slot.quantity = newVal;
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("", GUILayout.Width(60));
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("保存配置", GUILayout.Height(40)))
                SaveConfig();
            if (GUILayout.Button("清空选择", GUILayout.Height(24)))
                _config.slots.Clear();
        }

        private void SaveConfig()
        {
            PropDeployConfig existing = AssetDatabase.LoadAssetAtPath<PropDeployConfig>(ConfigPath);
            if (existing != null)
            {
                existing.slots = new List<PropSlotConfig>(_config.slots);
                EditorUtility.SetDirty(existing);
                AssetDatabase.SaveAssets();
            }
            else
            {
                string dir = Path.GetDirectoryName(ConfigPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                AssetDatabase.CreateAsset(_config, ConfigPath);
            }

            SetAddressable(ConfigPath, "PropDeployConfig");
            AssetDatabase.Refresh();
            Debug.Log("道具配置已保存");
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