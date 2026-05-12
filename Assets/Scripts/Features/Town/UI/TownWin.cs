using JKFrame;
using Main.Resource;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Town.UI
{
    [UIWindowDataAttribute(typeof(TownWin), true, "TownWin", 0)]
    public class TownWin : UI_WindowBase
    {
        [SerializeField] private TMP_Text _taxText;
        [SerializeField] private TMP_Text _woodAmountText;
        [SerializeField] private TMP_Text _stoneAmountText;
        [SerializeField] private TMP_Text _ironAmountText;
        [SerializeField] private TMP_Text _fabricAmountText;
        [SerializeField] private TMP_Text _goldAmountText;

        [SerializeField] private Button _buildBtn;
        [SerializeField] private Button _equipBtn;
        [SerializeField] private Button _recruitBtn;
        [SerializeField] private Button _heroBtn;
        [SerializeField] private Button _magicBtn;
        [SerializeField] private Button _merchantBtn;
        [SerializeField] private Button _dungeonBtn;

        public override void OnShow()
        {
            UpdateAllDisplay();
            EventSystem.AddEventListener("GoldChanged", OnGoldChanged);
        }

        public override void OnClose()
        {
            EventSystem.RemoveEventListener("GoldChanged", OnGoldChanged);
        }

        private void UpdateAllDisplay()
        {
            // TODO: 数据层就绪后替换为实际数据绑定（如 ResourceManager.Wood）
            _taxText.text = "税收  0";
            _woodAmountText.text = "0";
            _stoneAmountText.text = "0";
            _ironAmountText.text = "0";
            _fabricAmountText.text = "0";
            _goldAmountText.text = ResourceManager.Gold.ToString();
        }

        private void OnGoldChanged()
        {
            _goldAmountText.text = ResourceManager.Gold.ToString();
        }
    }
}