using System;
using System.Collections.Generic;
using Features.Town.Domain;
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

        private UnlockSource _source;
        private readonly List<EntryItemComponent> _itemComponents = new();

        public void Init(UnlockSource source, string title, GameObject itemPrefab, Action<RecruitEntry> onRecruit)
        {
            _source = source;
            _titleText.text = title;

            foreach (RecruitEntry entry in RecruitService.GetPool(source))
            {
                GameObject itemObj = Instantiate(itemPrefab, _itemsContainer);
                EntryItemComponent itemComp = itemObj.GetComponent<EntryItemComponent>();
                if (itemComp != null)
                {
                    itemComp.Init(entry);
                    _itemComponents.Add(itemComp);
                }
            }

            _chooseBtn.onClick.RemoveAllListeners();
            _chooseBtn.onClick.AddListener(() =>
            {
                RecruitEntry drawn = RecruitService.DrawFrom(_source);
                if (drawn != null)
                {
                    onRecruit?.Invoke(drawn);
                }
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