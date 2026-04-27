using System;
using JKFrame;
using UnityEngine;

namespace Main.GM
{
    public class GmSystem : MonoBehaviour, IGmCommandActions
    {
        private static GmSystem _instance;

        public static GmSystem Ins => _instance;

        [Header("GM Settings")]
        public int maxHistoryCount = 30;
        public int maxCommand = 34;

        public GmCommandRegistry Registry { get; private set; }
        public GmCommandHistory History { get; private set; }
        public GmCommandExecutor Executor { get; private set; }
        public GmHelpContentProvider HelpProvider { get; private set; }

        public bool IsOpen { get; private set; }

        private GmPanelInputCoordinator _inputCoordinator;
        private GmPanelWindow _window;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Registry = new GmCommandRegistry();
            History = new GmCommandHistory();
            HelpProvider = new GmHelpContentProvider();
            Executor = new GmCommandExecutor(Registry);

            _inputCoordinator = new GmPanelInputCoordinator(
                TogglePanel,
                NavigateHistoryUp,
                NavigateHistoryDown,
                ExecuteCurrentCommand,
                () =>
                {
                    GmPanelWindow window = GetWindow();
                    if (window != null)
                    {
                        window.InputField.ActivateInputField();
                    }
                });

            RegisterBuiltinCommands();
            OnPopulateCommands();
        }

        private void Update()
        {
            GmPanelWindow window = GetWindow();
            _inputCoordinator.Tick(IsOpen, window?.InputField);
        }

        private GmPanelWindow GetWindow()
        {
            if (_window == null || _window.gameObject == null)
            {
                _window = UISystem.GetWindow<GmPanelWindow>();
            }

            return _window;
        }

        protected virtual void OnPopulateCommands()
        {
        }

        private void RegisterBuiltinCommands()
        {
            Registry.Register("help", OnHelpCommand, "显示此帮助信息");
            Registry.Register("clear", OnClearCommand, "清空控制台");
        }

        private bool OnHelpCommand(string[] parts, IGmCommandActions actions)
        {
            foreach (string line in HelpProvider.GetHelpLines())
            {
                actions.ShowMessage(line);
            }

            foreach (GmCommandRegistry.CommandEntry entry in Registry.GetAllEntries())
            {
                string usage = string.IsNullOrEmpty(entry.Usage) ? "" : $" 用法: {entry.Usage}";
                actions.ShowMessage($"{entry.Command} - {entry.Description}{usage}");
            }

            return true;
        }

        private bool OnClearCommand(string[] parts, IGmCommandActions actions)
        {
            ClearConsole();
            return true;
        }

        public void RegisterCommand(string command, Func<string[], IGmCommandActions, bool> handler,
            string description, string usage = "")
        {
            Registry.Register(command, handler, description, usage);
        }

        public void UnregisterCommand(string command)
        {
            Registry.Unregister(command);
        }

        public void OnPanelOpened()
        {
            IsOpen = true;
            GmPanelWindow window = GetWindow();
            if (window == null) return;

            ClearConsole();
            ShowHelpInfo();
        }

        public void OnPanelClosed()
        {
            IsOpen = false;
            ClearConsole();
        }

        public void SubmitCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            string command = input.Trim();
            History.Add(command, maxHistoryCount);
            ShowMessage(command);
            Executor.Execute(command, this);

            GmPanelWindow window = GetWindow();
            if (window == null) return;

            window.InputField.text = "";
            History.ResetNavigation();
            window.InputField.ActivateInputField();
            window.InputField.Select();
        }

        private void TogglePanel()
        {
            if (IsOpen)
            {
                UISystem.Close<GmPanelWindow>();
            }
            else
            {
                UISystem.Show<GmPanelWindow>();
            }
        }

        private void NavigateHistoryUp()
        {
            if (!History.TryNavigateUp(out string command)) return;

            GmPanelWindow window = GetWindow();
            if (window == null) return;

            window.InputField.text = command;
            window.InputField.caretPosition = command.Length;
        }

        private void NavigateHistoryDown()
        {
            if (!History.TryNavigateDown(out string command)) return;

            GmPanelWindow window = GetWindow();
            if (window == null) return;

            window.InputField.text = command;
            window.InputField.caretPosition = command.Length;
        }

        private void ExecuteCurrentCommand()
        {
            GmPanelWindow window = GetWindow();
            if (window == null) return;

            if (!string.IsNullOrWhiteSpace(window.InputField.text))
            {
                SubmitCommand(window.InputField.text);
            }
        }

        private void ShowHelpInfo()
        {
            OnHelpCommand(null, this);
        }

        public void ShowMessage(string message)
        {
            GmPanelWindow window = GetWindow();
            window?.AppendCommand(message);
        }

        public void ClearConsole()
        {
            GmPanelWindow window = GetWindow();
            window?.ClearCommands();
        }
    }
}
