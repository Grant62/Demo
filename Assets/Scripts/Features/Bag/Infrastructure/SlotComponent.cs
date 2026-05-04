using Features.Bag.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Bag.Infrastructure
{
    [RequireComponent(typeof(Image))]
    public class SlotComponent : MonoBehaviour
    {
        [SerializeField] private EquipmentSlotType _slotType;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _qualityFrameImage;

        public EquipmentSlotType SlotType => _slotType;
        public BagItemData CurrentItem { get; private set; }
        public Image BackgroundImage { get; private set; }

        private void Awake()
        {
            BackgroundImage = GetComponent<Image>();
        }

        public void Setup(EquipmentSlotType slotType)
        {
            _slotType = slotType;
        }

        public void SetItem(BagItemData item, Sprite iconSprite, Sprite qualityFrameSprite)
        {
            CurrentItem = item;

            if (item == null)
            {
                if (_iconImage != null)
                {
                    _iconImage.sprite = null;
                    _iconImage.enabled = false;
                }
                if (_qualityFrameImage != null)
                {
                    _qualityFrameImage.sprite = null;
                    _qualityFrameImage.enabled = false;
                }
                return;
            }

            if (_iconImage != null)
            {
                _iconImage.sprite = iconSprite;
                _iconImage.enabled = true;
            }
            if (_qualityFrameImage != null)
            {
                _qualityFrameImage.sprite = qualityFrameSprite;
                _qualityFrameImage.enabled = true;
            }
        }

        public void SetHighlight(bool highlighted)
        {
            if (BackgroundImage != null)
            {
                Color c = BackgroundImage.color;
                c.a = highlighted ? 0.8f : 1f;
                BackgroundImage.color = c;
            }
        }
    }
}