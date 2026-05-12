using System;
using Configuration.ExcelData.DataClass;
using Features.Action.Application;
using Features.Action.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Action.UI
{
    [RequireComponent(typeof(Image))]
    public class MercenaryController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descText;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private Image _iconImage;

        private Button _button;

        public event Action<MercenaryController, int> OnClick;

        public MercenaryModel Model { get; private set; }

        public int Index { get; set; }

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_button == null)
            {
                _button = gameObject.AddComponent<Button>();
            }

            _button.onClick.AddListener(() => OnClick?.Invoke(this, Index));
            _button.transition = Selectable.Transition.None;
        }

        public void Initialize(OccupationInfo occupation, EntryInfo entry)
        {
            Model = MercenaryService.CreateModel(occupation, entry);
            UpdateUI();
        }

        public ActionResult Use(BattleContext context)
        {
            ActionResult result = MercenaryService.Execute(Model, Index, context);
            UpdateUI();
            return result;
        }

        public int GetForcedTarget(BattleContext context)
        {
            return MercenaryService.GetForcedTarget(Model, context);
        }

        public void UpdateUI()
        {
            _nameText.text = Model.DisplayName;
            _costText.text = Model.EnergyCost.ToString();

            string desc = Model.ProcessedDesc;
            if (Model.TenacityStacks > 0)
            {
                desc += $"\n<color=#00FF00>坚韧 x{Model.TenacityStacks}</color>";
            }

            if (Model.HasRevitalize)
            {
                desc += $"\n<color=#00FF00>回春 {Model.RevitalizeAmount}</color>";
            }

            if (Model.HasCounter)
            {
                desc += "\n<color=#FFA500>反击中</color>";
            }

            _descText.text = desc;
        }

        public void Setup(string name, string desc, int cost, Sprite icon)
        {
            _nameText.text = name;
            _descText.text = desc;
            _costText.text = cost.ToString();
            _iconImage.sprite = icon;
            _iconImage.enabled = icon != null;
        }
    }
}