using Features.CrawlerMap.Domain;
using UnityEditor;
using UnityEngine;

namespace Features.CrawlerMap.Editor
{
    public class CrawlerMapEditorWindow : EditorWindow
    {
        private CrawlerMapEditorState S;

        private const float BaseCellSize = 36f;

        private static readonly Color ColorEmptyA = new(0.18f, 0.18f, 0.20f, 0.35f);
        private static readonly Color ColorEmptyB = new(0.22f, 0.22f, 0.24f, 0.35f);
        private static readonly Color ColorWall = new(0f, 0f, 0f);
        private static readonly Color ColorSpace = new(0.85f, 0.85f, 0.85f);
        private static readonly Color ColorEntrance = new(1.00f, 0.84f, 0.00f);
        private static readonly Color ColorExit = new(1.00f, 0.65f, 0.10f);
        private static readonly Color ColorBoss = new(0.75f, 0.25f, 0.85f);
        private static readonly Color ColorEvent = new(0.30f, 0.55f, 1.00f);
        private static readonly Color ColorItem = new(0.30f, 0.90f, 0.35f);
        private static readonly Color ColorEnemy = new(0.95f, 0.30f, 0.30f);
        private static readonly Color ColorGridLine = new(0.40f, 0.40f, 0.40f, 0.50f);
        private static readonly Color ColorBlockPreview = new(1f, 1f, 0f, 0.25f);
        private static readonly Color ColorSelOutline = new(1f, 1f, 0f, 0.80f);

        private static readonly Color ColorBrushSpace = new(0.75f, 0.75f, 0.75f);
        private static readonly Color ColorBrushWall = new(0f, 0f, 0f);
        private static readonly Color ColorBrushEntrance = new(1.00f, 0.84f, 0.00f);
        private static readonly Color ColorBrushExit = new(1.00f, 0.65f, 0.10f);
        private static readonly Color ColorBrushBoss = new(0.75f, 0.25f, 0.85f);
        private static readonly Color ColorBrushErase = new(0.50f, 0.25f, 0.05f);
        private static readonly Color ColorBrushEvent = new(0.30f, 0.55f, 1.00f);
        private static readonly Color ColorBrushItem = new(0.30f, 0.90f, 0.35f);
        private static readonly Color ColorBrushEnemy = new(0.95f, 0.30f, 0.30f);

        private float CellSize { get => BaseCellSize * S.Zoom; }

        [MenuItem("游戏工具/爬行者地图编辑器")]
        private static void Open()
        {
            CrawlerMapEditorWindow window = GetWindow<CrawlerMapEditorWindow>("Crawler Map");
            window.minSize = new Vector2(600, 450);
            window.S = new CrawlerMapEditorState();
            window.Show();
        }

        private void OnEnable()
        {
            S ??= new CrawlerMapEditorState();
        }

        private void OnGUI()
        {
            DrawToolbar();
            DrawGeneratePanel();

            if (S.Data == null)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.HelpBox("请先创建或加载一个 CrawlerMapData 资产 (点击顶部 New / Load)", MessageType.Info);
                return;
            }

            EditorGUILayout.Space(4);

            Rect gridRect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawGridArea(gridRect);
            DrawInspector();
        }

        private void DrawToolbar()
        {
            Row1Toolbar();
            Row2Toolbar();
        }

        private void Row1Toolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("新建", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                CreateNewMap();
            }
            if (GUILayout.Button("保存", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                SaveMap();
            }
            if (GUILayout.Button("加载", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                LoadMap();
            }
            if (GUILayout.Button("生成", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                S.ShowGenSettings = !S.ShowGenSettings;
            }
            if (GUILayout.Button("清空", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                if (EditorUtility.DisplayDialog("Clear All", "确认清除所有格子?", "Yes", "No"))
                {
                    S.Data.ClearAll();
                    EditorUtility.SetDirty(S.Data);
                }
            }

            GUILayout.FlexibleSpace();

            GUILayout.Label($"缩放 {S.Zoom * 100:F0}%", EditorStyles.miniLabel);
            S.Zoom = GUILayout.HorizontalSlider(S.Zoom, 0.3f, 3f, GUILayout.Width(80));

            EditorGUILayout.EndHorizontal();
        }

        private void Row2Toolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            DrawBrushButton(CellContentType.Entrance, ColorBrushEntrance);
            DrawBrushButton(CellContentType.Exit, ColorBrushExit);
            DrawBrushButton(CellContentType.Boss, ColorBrushBoss);
            DrawBrushButton(CellContentType.Eraser, ColorBrushErase);
            DrawBrushButton(CellContentType.Event, ColorBrushEvent);
            DrawBrushButton(CellContentType.Item, ColorBrushItem);
            DrawBrushButton(CellContentType.Enemy, ColorBrushEnemy);

            GUILayout.Space(4);
            string brushName = S.Brush switch
            {
                CellContentType.Space => "区域",
                CellContentType.Entrance => "入口",
                CellContentType.Exit => "出口",
                CellContentType.Boss => "Boss",
                CellContentType.Wall => "墙",
                CellContentType.Eraser => "擦除",
                CellContentType.Event => "事件",
                CellContentType.Item => "物资",
                CellContentType.Enemy => "敌人",
                _ => "?",
            };
            string status = S.HasBrush ? brushName : "无画刷";
            GUILayout.Label(status, EditorStyles.miniLabel, GUILayout.Width(60));
            GUILayout.Space(4);

            DrawBrushButton(CellContentType.Space, ColorBrushSpace);
            DrawBlockToolbarButton(1, 1, "1x1");
            DrawBlockToolbarButton(2, 2, "2x2");
            DrawBlockToolbarButton(3, 3, "3x3");
            DrawBlockToolbarButton(2, 3, "2x3");
            DrawBlockToolbarButton(3, 2, "3x2");
            DrawBlockToolbarButton(4, 3, "4x3");
            DrawBlockToolbarButton(4, 4, "4x4");
            DrawBrushButton(CellContentType.Wall, ColorBrushWall);

            GUILayout.FlexibleSpace();

            string info = S.Data != null ? $"{S.Data.Width}x{S.Data.Height}" : "";
            GUILayout.Label(info, EditorStyles.miniLabel);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawBrushButton(CellContentType type, Color swatchColor)
        {
            bool isActive = S.HasBrush && S.Brush == type && S.BlockW == 1;
            string label = type switch
            {
                CellContentType.Space => "区域",
                CellContentType.Entrance => "入口",
                CellContentType.Exit => "出口",
                CellContentType.Boss => "Boss",
                CellContentType.Wall => "墙",
                CellContentType.Eraser => "擦除",
                CellContentType.Event => "事件",
                CellContentType.Item => "物资",
                CellContentType.Enemy => "敌人",
                _ => "?",
            };

            Color oldColor = GUI.color;
            if (isActive)
            {
                GUI.color = new Color(0.6f, 0.8f, 1f);
            }

            if (GUILayout.Button($"  {label}", EditorStyles.toolbarButton, GUILayout.Width(54)))
            {
                if (isActive && S.HasBrush)
                {
                    S.SavedBlockW = S.BlockW;
                    S.SavedBlockH = S.BlockH;
                    S.HasBrush = false;
                }
                else
                {
                    S.Brush = type;
                    if (type == CellContentType.Space)
                    {
                        S.BlockW = S.SavedBlockW;
                        S.BlockH = S.SavedBlockH;
                    }
                    else
                    {
                        S.BlockW = 1;
                        S.BlockH = 1;
                    }
                    S.HasBrush = true;
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                Rect swatch = new(lastRect.x + 3, lastRect.y + 3, 12, 12);
                EditorGUI.DrawRect(swatch, swatchColor);
                if (isActive)
                {
                    EditorGUI.DrawRect(new Rect(swatch.x - 1, swatch.y - 1, 14, 14), new Color(1f, 1f, 0f, 0.6f));
                }
            }

            GUI.color = oldColor;
        }

        private void DrawBlockToolbarButton(int w, int h, string label)
        {
            bool isActive = S.HasBrush && S.BlockW == w && S.BlockH == h && S.Brush == CellContentType.Space;

            Color old = GUI.color;
            if (isActive)
            {
                GUI.color = new Color(0.6f, 0.8f, 1f);
            }

            if (GUILayout.Button(label, EditorStyles.toolbarButton, GUILayout.Width(34)))
            {
                if (isActive && S.HasBrush)
                {
                    S.SavedBlockW = S.BlockW;
                    S.SavedBlockH = S.BlockH;
                    S.HasBrush = false;
                }
                else
                {
                    S.Brush = CellContentType.Space;
                    S.BlockW = w;
                    S.BlockH = h;
                    S.SavedBlockW = w;
                    S.SavedBlockH = h;
                    S.HasBrush = true;
                }
            }

            GUI.color = old;
        }

        private void DrawGeneratePanel()
        {
            if (!S.ShowGenSettings)
            {
                return;
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("地图生成", EditorStyles.boldLabel);

            S.GenSettings.seed = EditorGUILayout.IntField("种子 (0=随机)", S.GenSettings.seed);
            S.GenSettings.blockCount = EditorGUILayout.IntSlider("空间块数量", S.GenSettings.blockCount, 1, 50);

            float minW = S.GenSettings.minWidth;
            float maxW = S.GenSettings.maxWidth;
            EditorGUILayout.MinMaxSlider(
                $"宽度 [{S.GenSettings.minWidth}, {S.GenSettings.maxWidth}]",
                ref minW, ref maxW, 1f, 8f
            );
            S.GenSettings.minWidth = Mathf.RoundToInt(minW);
            S.GenSettings.maxWidth = Mathf.RoundToInt(maxW);

            float minH = S.GenSettings.minHeight;
            float maxH = S.GenSettings.maxHeight;
            EditorGUILayout.MinMaxSlider(
                $"高度 [{S.GenSettings.minHeight}, {S.GenSettings.maxHeight}]",
                ref minH, ref maxH, 1f, 8f
            );
            S.GenSettings.minHeight = Mathf.RoundToInt(minH);
            S.GenSettings.maxHeight = Mathf.RoundToInt(maxH);

            S.GenSettings.margin = EditorGUILayout.IntSlider("块间距", S.GenSettings.margin, 0, 3);
            S.GenSettings.fillWalls = EditorGUILayout.Toggle("剩余区域填充墙壁", S.GenSettings.fillWalls);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("生成", GUILayout.Height(26)))
            {
                if (S.Data == null)
                {
                    EditorUtility.DisplayDialog("提示", "请先 New 或 Load 一个地图资产", "OK");
                }
                else
                {
                    MapGenerator.Generate(S.Data, S.GenSettings);
                    EditorUtility.SetDirty(S.Data);
                    S.ShowGenSettings = false;
                }
            }

            if (GUILayout.Button("关闭", GUILayout.Height(26)))
            {
                S.ShowGenSettings = false;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawGridArea(Rect rect)
        {
            if (S.Data == null || rect.width < 10 || rect.height < 10)
            {
                return;
            }

            Event e = Event.current;
            Vector2 mousePos = e.mousePosition;
            bool mouseInRect = rect.Contains(mousePos);

            int hoverX = -1, hoverY = -1;
            if (mouseInRect)
            {
                MouseToCell(mousePos, rect, out hoverX, out hoverY);
            }

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (mouseInRect)
                    {
                        if (e.button == 0 && S.HasBrush && S.Data != null)
                        {
                            PaintCell(hoverX, hoverY);
                            e.Use();
                        }
                        else if (e.button == 2)
                        {
                            S.IsPanning = true;
                            S.PanStartMouse = mousePos;
                            S.PanStartOffset = S.PanOffset;
                            e.Use();
                        }
                    }

                    break;

                case EventType.MouseDrag:
                    if (mouseInRect && S.IsPanning)
                    {
                        Vector2 delta = (mousePos - S.PanStartMouse) / CellSize;
                        S.PanOffset = S.PanStartOffset + new Vector2(delta.x, -delta.y);
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (e.button == 0 && S.HasBrush && S.Data != null)
                    {
                        AssetDatabase.SaveAssets();
                    }

                    if (e.button == 2)
                    {
                        S.IsPanning = false;
                        e.Use();
                    }
                    else if (e.button == 1 && mouseInRect)
                    {
                        if (hoverX >= 0 && hoverY >= 0
                                        && hoverX < S.Data.Width && hoverY < S.Data.Height)
                        {
                            S.SelectCell(hoverX, hoverY);
                        }

                        e.Use();
                    }

                    break;

                case EventType.ScrollWheel:
                    if (mouseInRect)
                    {
                        float delta = -e.delta.y * 0.02f;
                        float oldZoom = S.Zoom;
                        S.Zoom = Mathf.Clamp(S.Zoom + delta, 0.3f, 3f);
                        float factor = S.Zoom / oldZoom;
                        Vector2 mouseToCenter = mousePos - rect.center;
                        S.PanOffset = S.PanOffset * factor
                                      - mouseToCenter / (CellSize * oldZoom) * (factor - 1f) * oldZoom;
                        e.Use();
                    }

                    break;

                case EventType.KeyDown:
                    if (S.ShowInspector && (e.keyCode == KeyCode.Escape || e.keyCode == KeyCode.Return))
                    {
                        S.ClearSelection();
                        e.Use();
                    }

                    break;
            }

            if (e.type == EventType.Repaint)
            {
                DrawGridContent(rect, hoverX, hoverY);
            }
        }

        private void DrawGridContent(Rect rect, int hoverX, int hoverY)
        {
            float cs = CellSize;
            Vector2 origin = new(
                rect.center.x + S.PanOffset.x * cs - S.Data.Width * cs * 0.5f - rect.x,
                rect.center.y - S.PanOffset.y * cs - S.Data.Height * cs * 0.5f - rect.y
            );

            GUI.BeginGroup(rect);

            for (int y = 0; y < S.Data.Height; y++)
            {
                for (int x = 0; x < S.Data.Width; x++)
                {
                    Rect cellRect = new(
                        origin.x + x * cs,
                        origin.y + y * cs,
                        cs, cs
                    );

                    if (!cellRect.Overlaps(new Rect(0, 0, rect.width, rect.height)))
                    {
                        continue;
                    }

                    CellData cell = S.Data.GetCell(x, y);
                    Color color = GetCellColor(cell, x, y);
                    EditorGUI.DrawRect(cellRect, color);

                    if (cell != null && cell.HasOverlay
                        && cell.OverlayType is CellContentType.Event
                            or CellContentType.Item
                            or CellContentType.Enemy)
                    {
                        Color oc = cell.OverlayType switch
                        {
                            CellContentType.Event => ColorEvent,
                            CellContentType.Item => ColorItem,
                            CellContentType.Enemy => ColorEnemy,
                            _ => Color.white,
                        };
                        Rect mark = new(cellRect.x + cs - 9, cellRect.y, 9, 9);
                        EditorGUI.DrawRect(mark, oc);

                    }

                    if (cell != null && cell.ContentType == CellContentType.Wall && cs > 8)
                    {
                        Color lineColor = new(0.35f, 0.35f, 0.35f);
                        float step = cs / 5f;
                        for (int i = 0; i <= 5; i++)
                        {
                            float t = i * step;
                            EditorGUI.DrawRect(
                                new Rect(cellRect.x + t, cellRect.y, 1, cellRect.height), lineColor);
                            EditorGUI.DrawRect(
                                new Rect(cellRect.x, cellRect.y + t, cellRect.width, 1), lineColor);
                        }
                    }


                }
            }

            for (int x = 0; x <= S.Data.Width; x++)
            {
                float px = origin.x + x * cs;
                EditorGUI.DrawRect(new Rect(px, origin.y, 1, S.Data.Height * cs), ColorGridLine);
            }

            for (int y = 0; y <= S.Data.Height; y++)
            {
                float py = origin.y + y * cs;
                EditorGUI.DrawRect(new Rect(origin.x, py, S.Data.Width * cs, 1), ColorGridLine);
            }

            if (hoverX >= 0 && hoverY >= 0)
            {
                Rect previewRect = new(
                    origin.x + hoverX * cs,
                    origin.y + hoverY * cs,
                    S.BlockW * cs,
                    S.BlockH * cs
                );
                EditorGUI.DrawRect(previewRect, ColorBlockPreview);
                EditorGUI.DrawRect(new Rect(previewRect.x, previewRect.y, previewRect.width, 1), ColorSelOutline);
                EditorGUI.DrawRect(new Rect(previewRect.x, previewRect.y + previewRect.height - 1, previewRect.width, 1), ColorSelOutline);
                EditorGUI.DrawRect(new Rect(previewRect.x, previewRect.y, 1, previewRect.height), ColorSelOutline);
                EditorGUI.DrawRect(new Rect(previewRect.x + previewRect.width - 1, previewRect.y, 1, previewRect.height), ColorSelOutline);
            }

            if (S.ShowInspector && S.SelX >= 0 && S.SelY >= 0)
            {
                Rect selRect = new(
                    origin.x + S.SelX * cs,
                    origin.y + S.SelY * cs,
                    cs, cs
                );
                for (int i = 0; i < 2; i++)
                {
                    EditorGUI.DrawRect(new Rect(selRect.x - i, selRect.y - i, selRect.width + i * 2, 1), ColorSelOutline);
                    EditorGUI.DrawRect(new Rect(selRect.x - i, selRect.y + selRect.height - 1 + i, selRect.width + i * 2, 1), ColorSelOutline);
                    EditorGUI.DrawRect(new Rect(selRect.x - i, selRect.y - i, 1, selRect.height + i * 2), ColorSelOutline);
                    EditorGUI.DrawRect(new Rect(selRect.x + selRect.width - 1 + i, selRect.y - i, 1, selRect.height + i * 2), ColorSelOutline);
                }
            }

            GUI.EndGroup();
        }

        private void PaintCell(int x, int y)
        {
            if (S.Data == null || x < 0 || y < 0 || x >= S.Data.Width || y >= S.Data.Height)
            {
                return;
            }

            if (S.Brush == CellContentType.Eraser)
            {
                for (int bx = 0; bx < S.BlockW; bx++)
                {
                    for (int by = 0; by < S.BlockH; by++)
                    {
                        CellData cell = S.Data.GetCell(x + bx, y + by);
                        if (cell == null)
                        {
                            continue;
                        }
                        if (cell.HasOverlay)
                        {
                            cell.OverlayType = CellContentType.Empty;
                            cell.ContentId = 0;
                            cell.ContentName = string.Empty;
                        }
                        else if (cell.ContentType != CellContentType.Empty)
                        {
                            cell.ContentType = CellContentType.Empty;
                            cell.OverlayType = CellContentType.Empty;
                            cell.ContentId = 0;
                            cell.ContentName = string.Empty;
                        }
                    }
                }

                EditorUtility.SetDirty(S.Data);
                return;
            }

            if (S.Brush == CellContentType.Space)
            {
                for (int bx = 0; bx < S.BlockW; bx++)
                {
                    for (int by = 0; by < S.BlockH; by++)
                    {
                        CellData cell = S.Data.GetCell(x + bx, y + by);
                        if (cell != null)
                        {
                            cell.ContentType = CellContentType.Space;
                            cell.OverlayType = CellContentType.Empty;
                            cell.ContentId = 0;
                            cell.ContentName = string.Empty;
                        }
                    }
                }
                EditorUtility.SetDirty(S.Data);
                return;
            }

            CellData target = S.Data.GetCell(x, y);
            if (target == null)
            {
                return;
            }

            switch (S.Brush)
            {
                case CellContentType.Space:
                    target.ContentType = CellContentType.Space;
                    target.OverlayType = CellContentType.Empty;
                    target.ContentId = 0;
                    target.ContentName = string.Empty;
                    break;

                case CellContentType.Wall:
                    target.ContentType = CellContentType.Wall;
                    target.OverlayType = CellContentType.Empty;
                    target.ContentId = 0;
                    target.ContentName = string.Empty;
                    break;

                case CellContentType.Entrance:
                case CellContentType.Exit:
                    target.ContentType = CellContentType.Space;
                    target.OverlayType = S.Brush;
                    target.ContentId = 0;
                    target.ContentName = string.Empty;
                    break;

                case CellContentType.Boss:
                    target.ContentType = CellContentType.Space;
                    target.OverlayType = S.Brush;
                    target.ContentId = 0;
                    target.ContentName = string.Empty;
                    break;

                default:
                    // Event / Item / Enemy: overlay on walkable, direct on other
                    if (target.ContentType == CellContentType.Space)
                    {
                        target.OverlayType = S.Brush;
                    }
                    else
                    {
                        target.ContentType = S.Brush;
                        target.OverlayType = CellContentType.Empty;
                    }
                    target.ContentId = 0;
                    target.ContentName = string.Empty;
                    break;
            }

            EditorUtility.SetDirty(S.Data);
        }

        private void MouseToCell(Vector2 mousePos, Rect rect, out int x, out int y)
        {
            float cs = CellSize;
            Vector2 origin = new(
                rect.center.x + S.PanOffset.x * cs - S.Data.Width * cs * 0.5f,
                rect.center.y - S.PanOffset.y * cs - S.Data.Height * cs * 0.5f
            );
            float gx = (mousePos.x - origin.x) / cs;
            float gy = (mousePos.y - origin.y) / cs;
            x = Mathf.FloorToInt(gx);
            y = Mathf.FloorToInt(gy);
        }

        private Color GetCellColor(CellData cell, int gridX, int gridY)
        {
            if (cell == null)
            {
                return ColorEmptyA;
            }

            CellContentType showType = cell.HasOverlay ? cell.OverlayType : cell.ContentType;

            switch (showType)
            {
                case CellContentType.Empty:
                    return (gridX + gridY) % 2 == 0 ? ColorEmptyA : ColorEmptyB;
                case CellContentType.Wall:
                    return ColorWall;
                case CellContentType.Space:
                    return ColorSpace;
                case CellContentType.Entrance:
                    return ColorEntrance;
                case CellContentType.Exit:
                    return ColorExit;
                case CellContentType.Boss:
                    return ColorBoss;
                case CellContentType.Event:
                    return ColorEvent;
                case CellContentType.Item:
                    return ColorItem;
                case CellContentType.Enemy:
                    return ColorEnemy;
                default:
                    return ColorEmptyA;
            }
        }

        private void DrawInspector()
        {
            if (!S.ShowInspector || S.Data == null)
            {
                return;
            }

            CellData cell = S.Data.GetCell(S.SelX, S.SelY);
            if (cell == null)
            {
                S.ClearSelection();
                return;
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField($"单元格 ({S.SelX}, {S.SelY})", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUILayout.EnumPopup("基底类型", cell.ContentType);
                if (cell.HasOverlay)
                {
                    EditorGUILayout.EnumPopup("叠放类型", cell.OverlayType);
                }
            }

            EditorGUI.BeginChangeCheck();

            int newId = EditorGUILayout.IntField("ContentId", cell.ContentId);
            string newName = EditorGUILayout.TextField("名称", cell.ContentName);

            if (EditorGUI.EndChangeCheck())
            {
                cell.ContentId = newId;
                cell.ContentName = newName;
                EditorUtility.SetDirty(S.Data);
            }

            // Color bar: base color + overlay indicator
            if (cell.ContentType != CellContentType.Empty && cell.ContentType != CellContentType.Wall)
            {
                Color baseColor = cell.ContentType switch
                {
                    CellContentType.Space => ColorSpace,
                    CellContentType.Entrance => ColorEntrance,
                    CellContentType.Exit => ColorExit,
                    CellContentType.Boss => ColorBoss,
                    _ => ColorSpace,
                };
                Rect colorRect = EditorGUILayout.GetControlRect(GUILayout.Height(6));
                EditorGUI.DrawRect(colorRect, baseColor);
                if (cell.HasOverlay)
                {
                    Color oc = cell.OverlayType switch
                    {
                        CellContentType.Event => ColorEvent,
                        CellContentType.Item => ColorItem,
                        CellContentType.Enemy => ColorEnemy,
                        _ => Color.white,
                    };
                    EditorGUI.DrawRect(new Rect(colorRect.x + colorRect.width - 20, colorRect.y, 20, 6), oc);
                }
            }

            if (S.BlockW > 1 || S.BlockH > 1)
            {
                EditorGUILayout.HelpBox($"当前使用 {S.BlockW}x{S.BlockH} 块模式放置 Space", MessageType.Info);
            }

            if (GUILayout.Button("关闭", GUILayout.Height(22)))
            {
                S.ClearSelection();
            }

            EditorGUILayout.EndVertical();
        }

        private static string GetContentLabel(CellContentType type)
        {
            return type switch
            {
                CellContentType.Empty => "空",
                CellContentType.Wall => "墙壁",
                CellContentType.Space => "空间",
                CellContentType.Entrance => "入口",
                CellContentType.Exit => "出口",
                CellContentType.Boss => "Boss",
                CellContentType.Event => "事件",
                CellContentType.Item => "物资",
                CellContentType.Enemy => "敌人",
                CellContentType.Eraser => "擦除",
                _ => "未知",
            };
        }

        private void CreateNewMap()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "新建 Crawler Map",
                "NewCrawlerMap",
                "asset",
                "选择保存位置"
            );

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            CrawlerMapData newData = CreateInstance<CrawlerMapData>();
            newData.Initialize(25, 25);
            AssetDatabase.CreateAsset(newData, path);
            AssetDatabase.SaveAssets();

            S.Data = newData;
            S.DataPath = path;
            S.PanOffset = Vector2.zero;
            S.Zoom = 1f;
            S.ClearSelection();
        }

        private void SaveMap()
        {
            if (S.Data == null)
            {
                EditorUtility.DisplayDialog("提示", "没有打开的地图数据", "OK");
                return;
            }

            EditorUtility.SetDirty(S.Data);
            AssetDatabase.SaveAssets();
            Debug.Log($"地图已保存: {AssetDatabase.GetAssetPath(S.Data)}");
        }

        private void LoadMap()
        {
            string path = EditorUtility.OpenFilePanel("加载 Crawler Map", "Assets", "asset");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string relativePath = GetRelativePath(path);
            CrawlerMapData loaded = AssetDatabase.LoadAssetAtPath<CrawlerMapData>(relativePath);
            if (loaded == null)
            {
                EditorUtility.DisplayDialog("错误", "无法加载所选文件", "OK");
                return;
            }

            S.Data = loaded;
            S.DataPath = relativePath;
            S.PanOffset = Vector2.zero;
            S.Zoom = 1f;
            S.ClearSelection();
            Debug.Log($"已加载地图: {relativePath}");
        }

        private static string GetRelativePath(string absolutePath)
        {
            string dataPath = Application.dataPath;
            if (absolutePath.StartsWith(dataPath))
            {
                return "Assets" + absolutePath[dataPath.Length..];
            }

            return absolutePath;
        }
    }
}