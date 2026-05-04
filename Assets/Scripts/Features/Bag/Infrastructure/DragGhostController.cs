using JKFrame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Bag.Infrastructure
{
    public class DragGhostController : MonoBehaviour
    {
        [SerializeField] private Image _ghostIcon;
        [SerializeField] private Image _ghostQualityFrame;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Show(Sprite iconSprite, Sprite qualityFrameSprite, Vector2 position)
        {
            if (_ghostIcon != null)
            {
                _ghostIcon.sprite = iconSprite;
                _ghostIcon.enabled = iconSprite != null;
            }
            if (_ghostQualityFrame != null)
            {
                _ghostQualityFrame.sprite = qualityFrameSprite;
                _ghostQualityFrame.enabled = qualityFrameSprite != null;
            }

            transform.SetParent(UISystem.DragLayer, false);
            _rectTransform.position = position;
            gameObject.SetActive(true);
        }

        public void FollowPointer(PointerEventData eventData)
        {
            _rectTransform.position = eventData.position;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            transform.SetParent(null);
        }
    }
}
