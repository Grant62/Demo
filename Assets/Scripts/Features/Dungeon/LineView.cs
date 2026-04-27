using UnityEngine;

namespace Features.Dungeon
{
    [RequireComponent(typeof(RectTransform))]
    public class LineView : MonoBehaviour
    {
        private RectTransform _rect;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        public void Connect(RectTransform from, RectTransform to)
        {
            Connect(from.anchoredPosition, to.anchoredPosition);
        }

        public void Connect(Vector2 fromPos, Vector2 toPos)
        {
            Vector2 direction = toPos - fromPos;
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            _rect.anchoredPosition = fromPos;
            _rect.sizeDelta = new Vector2(distance, _rect.sizeDelta.y);
            _rect.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
