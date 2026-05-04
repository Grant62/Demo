using Features.Bag.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Bag.Infrastructure
{
    [RequireComponent(typeof(Image))]
    public class ItemComponent : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _qualityFrameImage;
        [SerializeField] private TMP_Text _countText;

        public int BagIndex { get; private set; }
        public BagItemData ItemData { get; private set; }
        public Image BackgroundImage { get; private set; }

        private void Awake()
        {
            BackgroundImage = GetComponent<Image>();
        }

        public void Initialize(int bagIndex)
        {
            BagIndex = bagIndex;
        }

        public void SetItem(BagItemData item, Sprite iconSprite, Sprite qualityFrameSprite)
        {
            ItemData = item;

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

                if (_countText != null)
                {
                    _countText.text = string.Empty;
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

            if (_countText != null)
            {
                _countText.text = item.count > 1 ? $"x{item.count}" : string.Empty;
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