using Features.Dungeon;
using Features.Dungeon.Application;
using Features.Dungeon.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Dungeon.Infrastructure
{
    public class RoomViewFactory : IRoomViewFactory
    {
        private readonly GameObject _prefab;
        private readonly float _roomSize;

        public RoomViewFactory(GameObject prefab, DungeonSettings settings)
        {
            _prefab = prefab;
            _roomSize = settings.RoomSize;
        }

        public RoomView Create(RoomData data, Vector2 position, RectTransform container)
        {
            GameObject go;
            if (_prefab != null)
            {
                go = Object.Instantiate(_prefab, container);
            }
            else
            {
                go = new GameObject($"Room_{data.Position.X}_{data.Position.Y}", typeof(RectTransform), typeof(Image));
                go.transform.SetParent(container, false);
            }

            var roomView = go.GetComponent<RoomView>();
            if (roomView == null)
                roomView = go.AddComponent<RoomView>();

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(_roomSize, _roomSize);
            rt.anchoredPosition = position;

            if (go.GetComponent<Button>() == null && go.GetComponent<Image>() != null)
            {
                var button = go.AddComponent<Button>();
                button.targetGraphic = go.GetComponent<Image>();
                button.transition = Selectable.Transition.ColorTint;
                var colors = button.colors;
                colors.highlightedColor = new Color(1f, 1f, 0.5f, 1f);
                colors.selectedColor = new Color(1f, 1f, 0f, 1f);
                button.colors = colors;
            }

            return roomView;
        }
    }
}
