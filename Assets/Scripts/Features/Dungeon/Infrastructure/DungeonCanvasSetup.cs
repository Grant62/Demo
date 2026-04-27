using Features.Dungeon;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Dungeon.Infrastructure
{
    public class DungeonCanvasSetup
    {
        private const string DungeonRootName = "DungeonRoot";
        private const string LinesName = "Lines";
        private const string RoomsName = "Rooms";

        public RectTransform RoomsContainer { get; private set; }
        public RectTransform LinesContainer { get; private set; }

        private Canvas _canvas;

        public void EnsureCanvas()
        {
            _canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (_canvas == null)
            {
                var canvasGO = new GameObject("DungeonCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                _canvas = canvasGO.GetComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                var scaler = _canvas.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
            }

            if (GameObject.FindFirstObjectByType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }

            var rootTransform = CreateOrGetChild(_canvas.transform, DungeonRootName, true);
            LinesContainer = CreateOrGetChild(rootTransform, LinesName, false);
            RoomsContainer = CreateOrGetChild(rootTransform, RoomsName, false);
        }

        public void ClearGenerated()
        {
            var existing = GameObject.FindFirstObjectByType<DungeonRoot>();
            if (existing != null)
                Object.DestroyImmediate(existing.gameObject);
        }

        private static RectTransform CreateOrGetChild(Transform parent, string name, bool stretch)
        {
            var existing = parent.Find(name);
            if (existing != null)
                return existing.GetComponent<RectTransform>();

            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();

            if (stretch)
            {
                var root = go.AddComponent<DungeonRoot>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            return rt;
        }
    }
}
