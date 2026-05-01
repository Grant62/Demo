namespace Main.GM
{
    public class GmCommandExecutor
    {
        private readonly GmCommandRegistry _registry;

        public GmCommandExecutor(GmCommandRegistry registry)
        {
            _registry = registry;
        }

        public void Execute(string command, IGmCommandActions actions)
        {
            string[] parts = command.Split(' ');
            if (parts.Length == 0) return;

            string cmd = parts[0].ToLowerInvariant();

            if (!_registry.TryGetEntry(cmd, out GmCommandRegistry.CommandEntry entry))
            {
                actions.ShowMessage($"未知指令: {cmd}，输入 'help' 查看可用指令");
                return;
            }

            entry.Handler(parts, actions);
        }
    }
}