using System;
using TMPro;
using UnityEngine;

namespace Main.GM
{
    public class GmPanelInputCoordinator
    {
        private readonly Action _togglePanel;
        private readonly Action _navigateHistoryUp;
        private readonly Action _navigateHistoryDown;
        private readonly Action _executeCurrentCommand;
        private readonly Action _activateInputField;

        public GmPanelInputCoordinator(
            Action togglePanel,
            Action navigateHistoryUp,
            Action navigateHistoryDown,
            Action executeCurrentCommand,
            Action activateInputField)
        {
            _togglePanel = togglePanel;
            _navigateHistoryUp = navigateHistoryUp;
            _navigateHistoryDown = navigateHistoryDown;
            _executeCurrentCommand = executeCurrentCommand;
            _activateInputField = activateInputField;
        }

        public void Tick(bool gmIsOpen, TMP_InputField inputField)
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
                _togglePanel();

            if (!gmIsOpen || inputField == null)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (inputField.isFocused)
                {
                    if (!string.IsNullOrEmpty(inputField.text))
                    {
                        inputField.text = "";
                        inputField.ActivateInputField();
                    }
                    else
                    {
                        _togglePanel();
                    }

                    return;
                }

                _togglePanel();
                return;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (!inputField.isFocused)
                    _activateInputField();
                _navigateHistoryUp();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (!inputField.isFocused)
                    _activateInputField();
                _navigateHistoryDown();
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                _executeCurrentCommand();
            }
        }
    }
}