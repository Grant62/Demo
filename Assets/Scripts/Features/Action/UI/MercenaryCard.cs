using Configuration.ExcelData.DataClass;
using Features.Action.Application;
using Features.Action.Domain;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Features.Action.UI
{
    [RequireComponent(typeof(Image))]
    public class MercenaryCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descText;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private Image _iconImage;

        private Button _button;
        private MercenaryModel _model;

        public event System.Action<MercenaryCard, int> OnClick;

        public MercenaryModel Model => _model;
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
            _model = MercenaryService.CreateModel(occupation, entry);
            UpdateUI();
        }

        public ActionResult Use(BattleContext context)
        {
            ActionResult result = MercenaryService.Execute(_model, Index, context);
            UpdateUI();
            return result;
        }

        public int GetForcedTarget(BattleContext context)
        {
            return MercenaryService.GetForcedTarget(_model, context);
        }

        public void UpdateUI()
        {
            _nameText.text = _model.DisplayName;
            _costText.text = _model.EnergyCost.ToString();

            string desc = _model.DisplayDesc;
            if (_model.TenacityStacks > 0)
            {
                desc += $"\n<color=#00FF00>坚韧 x{_model.TenacityStacks}</color>";
            }
            if (_model.HasRevitalize)
            {
                desc += $"\n<color=#00FF00>回春 {_model.RevitalizeAmount}</color>";
            }
            if (_model.HasCounter)
            {
                desc += $"\n<color=#FFA500>反击中</color>";
            }
            _descText.text = desc;

            _iconImage.sprite = null;
            _iconImage.enabled = false;
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
