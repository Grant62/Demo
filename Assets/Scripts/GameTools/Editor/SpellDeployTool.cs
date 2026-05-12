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
    public class SpellDeployTool : EditorWindow
    {
        private const string ConfigPath = "Assets/GameResource/Config/SpellDeployConfig.asset";

        private List<SpellInfo> _allSpells;
        private SpellDeployConfig _config;
        private Vector2 _scrollPos;

        private void OnEnable()
        {
            LoadSpells();
            _config = AssetDatabase.LoadAssetAtPath<SpellDeployConfig>(ConfigPath);
            if (_config == null)
            {
                _config = CreateInstance<SpellDeployConfig>();
                _config.spellIds = new List<int>();
            }
        }

        private void LoadSpells()
        {
            SpellInfoContainer container = BinaryDataMgr.Ins.GetTable<SpellInfoContainer>();
            if (container == null) return;
            _allSpells = new List<SpellInfo>(container.DataDic.Values);
        }

        private void OnGUI()
        {
            if (_allSpells == null)
            {
                EditorGUILayout.HelpBox("请先构建二进制配置表", MessageType.Warning);
                return;
            }

            int selected = _config.spellIds.Count;
            EditorGUILayout.LabelField($"已选 {selected}", EditorStyles.boldLabel);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("法术名称", EditorStyles.boldLabel, GUILayout.Width(80));
            EditorGUILayout.LabelField("MP消耗", EditorStyles.boldLabel, GUILayout.Width(60));
            EditorGUILayout.LabelField("效果描述", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            foreach (SpellInfo spell in _allSpells)
            {
                bool isSelected = _config.spellIds.Contains(spell.Id);

                EditorGUILayout.BeginHorizontal();
                bool wasSelected = isSelected;
                isSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));

                if (isSelected != wasSelected)
                {
                    if (isSelected)
                        _config.spellIds.Add(spell.Id);
                    else
                        _config.spellIds.Remove(spell.Id);
                }

                EditorGUILayout.LabelField(spell.Name, GUILayout.Width(80));
                EditorGUILayout.LabelField(spell.Cost.ToString(), GUILayout.Width(60));
                GUILayout.Space(10);
                EditorGUILayout.LabelField(spell.Desc);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("保存配置", GUILayout.Height(40)))
                SaveConfig();
            if (GUILayout.Button("清空选择", GUILayout.Height(24)))
                _config.spellIds.Clear();
        }

        private void SaveConfig()
        {
            SpellDeployConfig existing = AssetDatabase.LoadAssetAtPath<SpellDeployConfig>(ConfigPath);
            if (existing != null)
            {
                existing.spellIds = new List<int>(_config.spellIds);
                EditorUtility.SetDirty(existing);
                AssetDatabase.SaveAssets();
            }
            else
            {
                string dir = Path.GetDirectoryName(ConfigPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                AssetDatabase.CreateAsset(_config, ConfigPath);
            }

            SetAddressable(ConfigPath, "SpellDeployConfig");
            AssetDatabase.Refresh();
            Debug.Log($"法术配置已保存");
        }

        private static void SetAddressable(string assetPath, string address)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                settings = AddressableAssetSettings.Create(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder, AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true, true);
                AddressableAssetSettingsDefaultObject.Settings = settings;
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
