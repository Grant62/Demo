using JKFrame;
using Main.Resource;
using TMPro;
using UnityEngine;

namespace Main.UI
{
    [UIWindowDataAttribute(typeof(GoldWin), true, "GoldWin", 0)]
    public class GoldWin : UI_WindowBase
    {
        [SerializeField] private TMP_Text _goldText;

        public override void OnShow()
        {
            UpdateGoldDisplay();
            EventSystem.AddEventListener("GoldChanged", UpdateGoldDisplay);
        }

        public override void OnClose()
        {
            EventSystem.RemoveEventListener("GoldChanged", UpdateGoldDisplay);
        }

        private void UpdateGoldDisplay()
        {
            _goldText.text = $"金币：{ResourceManager.Gold}";
        }
    }
}