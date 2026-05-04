using Features.Bag.UI;
using JKFrame;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Battle.UI
{
    [UIWindowDataAttribute(typeof(BattlePanelWin), false, "BattlePanel", 1)]
    public class BattlePanelWin : UI_WindowBase
    {
        [SerializeField] private Button _bagBtn;

        public override void OnShow()
        {
            _bagBtn.onClick.AddListener(OnBagClick);
        }

        public override void OnClose()
        {
            _bagBtn.onClick.RemoveAllListeners();
        }

        private void OnBagClick()
        {
            UISystem.Show<BagPanelWin>();
        }
    }
}