using Features.Dungeon.Domain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Dungeon
{
    public class RoomView : MonoBehaviour, IPointerClickHandler
    {
        public int GridX { get; set; }
        public int GridY { get; set; }
        public RoomType RoomType { get; set; }

        private Image _image;
        private Color _originalColor;

        private void Awake()
        {
            _image = GetComponent<Image>();
            if (_image == null)
                _image = gameObject.AddComponent<Image>();
            _originalColor = _image.color;
        }

        public void Setup(int x, int y, RoomType type)
        {
            GridX = x;
            GridY = y;
            RoomType = type;

            switch (type)
            {
                case RoomType.Entrance:
                    _image.color = Color.green;
                    break;
                case RoomType.Exit:
                    _image.color = Color.red;
                    break;
                default:
                    _image.color = Color.gray;
                    break;
            }

            _originalColor = _image.color;
        }

        public void SetSprite(Sprite sprite)
        {
            if (_image != null && sprite != null)
                _image.sprite = sprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"[Room] Clicked: ({GridX}, {GridY}) - {RoomType}");
        }
    }
}