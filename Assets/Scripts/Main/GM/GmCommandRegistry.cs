using System;
using System.Collections.Generic;

namespace Main.GM
{
    public class GmCommandRegistry
    {
        private readonly Dictionary<string, CommandEntry> _entries = new();

        public void Register(string command, Func<string[], IGmCommandActions, bool> handler, string description, string usage = "")
        {
            string key = command.ToLowerInvariant();
            _entries[key] = new CommandEntry
            {
                Command = command,
                Handler = handler,
                Description = description,
                Usage = usage
            };
        }

        public void Unregister(string command)
        {
            _entries.Remove(command.ToLowerInvariant());
        }

        public bool TryGetEntry(string command, out CommandEntry entry)
        {
            return _entries.TryGetValue(command.ToLowerInvariant(), out entry);
        }

        public IEnumerable<CommandEntry> GetAllEntries()
        {
            return _entries.Values;
        }

        public void Clear()
        {
            _entries.Clear();
        }

        public int Count => _entries.Count;

        public class CommandEntry
        {
            public string Command;
            public Func<string[], IGmCommandActions, bool> Handler;
            public string Description;
            public string Usage;
        }
    }
}
