using System;
using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Features.Town.Domain;
using JKFrame;
using Main.Resource;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Town.UI
{
    public class EntryComponent : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private RectTransform _itemsContainer;
        [SerializeField] private Button _chooseBtn;
        [SerializeField] private Image _buildingIconImage;

        private int _buildingId;
        private int _recruitmentFee;
        private readonly List<EntryItemComponent> _itemComponents = new();

        public void Init(int buildingId, string title, int recruitmentFee, string resAddress, GameObject itemPrefab, Action<OccupationInfo> onRecruit)
        {
            _buildingId = buildingId;
            _recruitmentFee = recruitmentFee;
            _titleText.text = $"{title}  招募费用：{recruitmentFee}";

            if (_buildingIconImage != null && !string.IsNullOrEmpty(resAddress))
            {
                _buildingIconImage.sprite = ResSystem.LoadAsset<Sprite>(resAddress);
            }

            // 目前所有建筑等级为 1
            int level = 1;
            List<OccupationInfo> occupations = RecruitService.GetOccupations(buildingId, level);
            foreach (OccupationInfo info in occupations)
            {
                GameObject itemObj = Instantiate(itemPrefab, _itemsContainer);
                EntryItemComponent itemComp = itemObj.GetComponent<EntryItemComponent>();
                if (itemComp != null)
                {
                    itemComp.Init(info);
                    _itemComponents.Add(itemComp);
                }
            }

            _chooseBtn.onClick.RemoveAllListeners();
            _chooseBtn.onClick.AddListener(() =>
            {
                OccupationInfo drawn = RecruitService.DrawFrom(_buildingId, level);
                if (drawn == null)
                {
                    return;
                }

                if (!ResourceManager.TrySpendGold(_recruitmentFee))
                {
                    Debug.LogWarning($"金币不足，需要 {_recruitmentFee}");
                    return;
                }

                onRecruit?.Invoke(drawn);
            });
        }

        public void Clear()
        {
            _chooseBtn.onClick.RemoveAllListeners();
            foreach (EntryItemComponent item in _itemComponents)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }

            _itemComponents.Clear();
        }
    }
}