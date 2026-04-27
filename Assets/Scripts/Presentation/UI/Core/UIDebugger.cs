using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Presentation.UI.Core
{
    public class UIDebugger : MonoBehaviour
    {
        [SerializeField] private bool enableLog = true;
        [SerializeField] private bool logEveryFrame;

        private void Update()
        {
            if (!enableLog) return;

            if (logEveryFrame)
            {
                LogRaycastAtMousePosition();
            }

            if (Input.GetMouseButtonDown(0))
            {
                LogRaycastAtMousePosition();
            }
        }

        private void LogRaycastAtMousePosition()
        {
            if (EventSystem.current == null)
            {
                Debug.LogError("[UIDebugger] EventSystem.current is null!");
                return;
            }

            PointerEventData pointerData = new(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count == 0)
            {
                Debug.Log($"[UIDebugger] No UI hit at {Input.mousePosition}");
                return;
            }

            StringBuilder sb = new();
            sb.AppendLine($"[UIDebugger] {results.Count} UI hit(s) at {Input.mousePosition}:");
            sb.AppendLine("---");

            for (int i = 0; i < results.Count; i++)
            {
                RaycastResult result = results[i];
                GameObject go = result.gameObject;
                int depth = CalculateDepth(go.transform);
                bool hasButton = go.GetComponent<Button>() != null;
                int canvasSortingOrder = result.sortingOrder;
                Canvas canvas = go.GetComponentInParent<Canvas>();
                string canvasName = canvas != null ? canvas.gameObject.name : "N/A";
                int canvasSO = canvas != null ? canvas.sortingOrder : -1;
                bool hasRaycaster = go.GetComponentInParent<GraphicRaycaster>() != null;

                sb.AppendLine($"  [{i}] \"{go.name}\"");
                sb.AppendLine($"       Button: {hasButton} | Depth: {depth} | HitSortingOrder: {canvasSortingOrder} | CanvasSortingOrder: {canvasSO}");
                sb.AppendLine($"       Canvas: \"{canvasName}\" | GraphicRaycaster: {hasRaycaster}");
                sb.AppendLine($"       Scene: {go.scene.name}");
                sb.AppendLine($"       Path: {GetHierarchyPath(go.transform)}");
            }

            Debug.Log(sb.ToString());
        }

        private static int CalculateDepth(Transform t)
        {
            int depth = 0;
            while (t.parent != null)
            {
                depth++;
                t = t.parent;
            }

            return depth;
        }

        private static string GetHierarchyPath(Transform t)
        {
            List<string> parts = new();
            while (t != null)
            {
                parts.Insert(0, t.name);
                t = t.parent;
            }

            return string.Join(" > ", parts);
        }
    }
}