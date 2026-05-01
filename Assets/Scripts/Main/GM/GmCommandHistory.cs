using System.Collections.Generic;

namespace Main.GM
{
    public class GmCommandHistory
    {
        private readonly List<string> _commandHistory = new();
        private int _historyIndex = -1;

        public void Add(string command, int maxHistoryCount)
        {
            if (_commandHistory.Count == 0 || _commandHistory[^1] != command)
            {
                _commandHistory.Add(command);
            }

            if (_commandHistory.Count > maxHistoryCount)
            {
                _commandHistory.RemoveAt(0);
            }

            _historyIndex = -1;
        }

        public bool TryNavigateUp(out string command)
        {
            command = null;
            if (_commandHistory.Count <= 0)
            {
                return false;
            }

            if (_historyIndex < _commandHistory.Count - 1)
            {
                _historyIndex++;
                int targetIndex = _commandHistory.Count - 1 - _historyIndex;
                command = _commandHistory[targetIndex];
                return true;
            }

            return false;
        }

        public bool TryNavigateDown(out string command)
        {
            command = null;
            if (_historyIndex > 0)
            {
                _historyIndex--;
                int targetIndex = _commandHistory.Count - 1 - _historyIndex;
                command = _commandHistory[targetIndex];
                return true;
            }

            if (_historyIndex == 0)
            {
                _historyIndex = -1;
                command = string.Empty;
                return true;
            }

            return false;
        }

        public void ResetNavigation()
        {
            _historyIndex = -1;
        }

        public void Clear()
        {
            _commandHistory.Clear();
            _historyIndex = -1;
        }
    }
}