using Features.Dungeon;
using Features.Dungeon.Application;
using Features.Dungeon.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Dungeon.Infrastructure
{
    public class LineViewFactory : ILineViewFactory
    {
        private readonly GameObject _prefab;
        private readonly float _halfRoom;

        public LineViewFactory(GameObject prefab, DungeonSettings settings)
        {
            _prefab = prefab;
            _halfRoom = settings.RoomSize * 0.5f;
        }

        public LineView Create(LineData data, Vector2 fromPos, Vector2 toPos, RectTransform container)
        {
            GameObject go;
            if (_prefab != null)
            {
                go = Object.Instantiate(_prefab, container);
            }
            else
            {
                go = new GameObject("Line", typeof(RectTransform), typeof(Image));
                go.transform.SetParent(container, false);
                var img = go.GetComponent<Image>();
                img.color = Color.white;
            }

            go.name = $"Line_{data.From.X}_{data.From.Y}_{data.To.X}_{data.To.Y}";

            var lineView = go.GetComponent<LineView>();
            if (lineView == null)
                lineView = go.AddComponent<LineView>();

            float halfRoom = _halfRoom;
            Vector2 dir = (toPos - fromPos).normalized;
            Vector2 adjustedFrom = fromPos + dir * halfRoom;
            Vector2 adjustedTo = toPos - dir * halfRoom;

            lineView.Connect(adjustedFrom, adjustedTo);
            return lineView;
        }
    }
}
