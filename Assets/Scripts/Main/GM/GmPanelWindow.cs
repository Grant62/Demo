using JKFrame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.GM
{
    [UIWindowDataAttribute(typeof(GmPanelWindow), true, "UI/GMPanel", 4)]
    public class GmPanelWindow : UI_WindowBase
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private RectTransform commandList;
        [SerializeField] private TMP_Text commandLineTemplate;

        private GmConsoleView _consoleView;

        public TMP_InputField InputField => inputField;

        public GmConsoleView ConsoleView => _consoleView;

        public override void Init()
        {
            base.Init();
            _consoleView = new GmConsoleView(commandList, commandLineTemplate,
                GmSystem.Ins?.maxCommand ?? 34);
        }

        public override void OnShow()
        {
            base.OnShow();
            transform.SetParent(UISystem.DragLayer, false);
            transform.SetAsLastSibling();
            Time.timeScale = 0f;

            inputField.ActivateInputField();
            inputField.Select();

            inputField.onEndEdit.AddListener(OnInputEndEdit);
            inputField.onSubmit.AddListener(OnSubmitCommand);
            inputField.lineType = TMP_InputField.LineType.SingleLine;

            if (GmSystem.Ins != null)
            {
                GmSystem.Ins.OnPanelOpened();
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            Time.timeScale = 1f;

            inputField.onEndEdit.RemoveListener(OnInputEndEdit);
            inputField.onSubmit.RemoveListener(OnSubmitCommand);
            inputField.DeactivateInputField();

            if (GmSystem.Ins != null)
            {
                GmSystem.Ins.OnPanelClosed();
            }
        }

        public void AppendCommand(string text)
        {
            _consoleView?.Append(text);
        }

        public void ClearCommands()
        {
            _consoleView?.ClearAll();
        }

        private void OnInputEndEdit(string input)
        {
        }

        private void OnSubmitCommand(string input)
        {
            if (GmSystem.Ins != null)
            {
                GmSystem.Ins.SubmitCommand(input);
            }
        }
    }
}
