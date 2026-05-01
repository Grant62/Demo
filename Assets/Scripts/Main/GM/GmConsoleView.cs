using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.GM
{
    public class GmConsoleView
    {
        private readonly RectTransform _contentArea;
        private readonly TMP_Text _lineTemplate;
        private readonly List<TMP_Text> _activeLines = new();
        private readonly Stack<TMP_Text> _pool = new();
        private ScrollRect _scrollRect;
        private int _maxLines;

        public GmConsoleView(RectTransform contentArea, TMP_Text lineTemplate, int maxLines = 34)
        {
            _contentArea = contentArea;
            _lineTemplate = lineTemplate;
            _maxLines = maxLines;
            TryEnsureScrollRect();
        }

        public void Append(string text)
        {
            if (_contentArea == null) return;

            TryEnsureScrollRect();

            TMP_Text line = GetLine();
            line.text = $"> {text}";
            line.gameObject.SetActive(true);
            _activeLines.Add(line);

            while (_activeLines.Count > _maxLines)
            {
                RecycleLine(_activeLines[0]);
                _activeLines.RemoveAt(0);
            }

            Canvas.ForceUpdateCanvases();
            TryScrollToBottom();
        }

        public void ClearAll()
        {
            foreach (TMP_Text line in _activeLines)
            {
                RecycleLine(line);
            }

            _activeLines.Clear();
        }

        public void SetMaxLines(int maxLines)
        {
            _maxLines = maxLines;
            while (_activeLines.Count > _maxLines)
            {
                RecycleLine(_activeLines[0]);
                _activeLines.RemoveAt(0);
            }
        }

        private TMP_Text GetLine()
        {
            if (_pool.Count > 0)
            {
                TMP_Text line = _pool.Pop();
                line.transform.SetAsLastSibling();
                return line;
            }

            TMP_Text newLine = Object.Instantiate(_lineTemplate, _contentArea);
            newLine.name = "GmCommandLine";
            return newLine;
        }

        private void RecycleLine(TMP_Text line)
        {
            line.gameObject.SetActive(false);
            line.transform.SetAsFirstSibling();
            _pool.Push(line);
        }

        private void TryScrollToBottom()
        {
            if (_scrollRect == null) return;

            if (_scrollRect.content == null)
            {
                _scrollRect.content = _contentArea;
            }

            _scrollRect.verticalNormalizedPosition = 0f;
        }

        private void TryEnsureScrollRect()
        {
            if (_scrollRect == null || _scrollRect.gameObject == null)
            {
                _scrollRect = _contentArea.GetComponentInParent<ScrollRect>();
                if (_scrollRect != null && _scrollRect.content == null)
                {
                    _scrollRect.content = _contentArea;
                }
            }
        }
    }
}