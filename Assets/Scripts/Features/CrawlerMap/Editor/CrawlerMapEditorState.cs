using Features.CrawlerMap.Domain;
using UnityEngine;

namespace Features.CrawlerMap.Editor
{
    public class CrawlerMapEditorState
    {
        public CrawlerMapData Data;
        public string DataPath;

        public CellContentType Brush = CellContentType.Space;
        public int BlockW = 1;
        public int BlockH = 1;
        public int SavedBlockW = 1;
        public int SavedBlockH = 1;
        public bool HasBrush;

        public float Zoom = 1f;
        public Vector2 PanOffset = Vector2.zero;
        public bool IsPanning;
        public Vector2 PanStartMouse;
        public Vector2 PanStartOffset;

        public int SelX = -1;
        public int SelY = -1;
        public bool ShowInspector;

        public int LastPaintX = -1;
        public int LastPaintY = -1;
        public bool IsDragging;

        public bool ShowGenSettings;
        public GenerationSettings GenSettings = GenerationSettings.Default();

        public void SelectCell(int x, int y)
        {
            SelX = x;
            SelY = y;
            ShowInspector = true;
        }

        public void ClearSelection()
        {
            SelX = -1;
            SelY = -1;
            ShowInspector = false;
        }
    }
}