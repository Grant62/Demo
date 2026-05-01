using System.Collections.Generic;

namespace Main.GM
{
    public class GmHelpContentProvider
    {
        private readonly List<string> _helpLines = new()
        {
            "=== GM Console ===",
            "按 ~ 打开/关闭控制台，↑↓ 浏览历史",
            "help - 显示此帮助信息",
            "clear - 清空控制台"
        };

        public void AddHelpLine(string line)
        {
            _helpLines.Add(line);
        }

        public void AddHelpLines(IEnumerable<string> lines)
        {
            _helpLines.AddRange(lines);
        }

        public void RemoveHelpLine(string line)
        {
            _helpLines.Remove(line);
        }

        public void ClearHelpLines()
        {
            _helpLines.Clear();
        }

        public IReadOnlyList<string> GetHelpLines()
        {
            return _helpLines;
        }
    }
}