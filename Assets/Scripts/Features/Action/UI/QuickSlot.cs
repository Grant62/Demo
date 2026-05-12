using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Action.UI
{
    [RequireComponent(typeof(Image))]
    public class QuickSlot : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _amountText;

        private Button _button;

        public event Action<int> OnClick;

        public int Index { get; set; }

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_button == null)
                _button = gameObject.AddComponent<Button>();
            _button.onClick.AddListener(() => OnClick?.Invoke(Index));
            _button.transition = Selectable.Transition.None;
        }

        public void Setup(Sprite icon, int index)
        {
            _iconImage.sprite = icon;
            _iconImage.enabled = icon != null;
            Index = index;
            if (_amountText != null)
                _amountText.text = "";
        }

        public void SetupWithAmount(Sprite icon, int quantity, int index)
        {
            _iconImage.sprite = icon;
            _iconImage.enabled = icon != null;
            Index = index;
            if (_amountText != null)
                _amountText.text = quantity > 1 ? quantity.ToString() : "";
        }

        public void SetupWithMpCost(Sprite icon, int mpCost, int index)
        {
            _iconImage.sprite = icon;
            _iconImage.enabled = icon != null;
            Index = index;
            if (_amountText != null)
                _amountText.text = $"MP:{mpCost}";
        }
    }
}