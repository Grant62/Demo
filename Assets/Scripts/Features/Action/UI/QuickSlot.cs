using UnityEngine;
using UnityEngine.UI;

namespace Features.Action.UI
{
    [RequireComponent(typeof(Image))]
    public class QuickSlot : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;

        private Button _button;
        private int _index;

        public event System.Action<int> OnClick;

        public int Index
        {
            get => _index;
            set => _index = value;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_button == null)
            {
                _button = gameObject.AddComponent<Button>();
            }
            _button.onClick.AddListener(() => OnClick?.Invoke(_index));
            _button.transition = Selectable.Transition.None;
        }

        public void Setup(Sprite icon, int index)
        {
            _iconImage.sprite = icon;
            _iconImage.enabled = icon != null;
            _index = index;
        }
    }
}
