using JKFrame;
using Main.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Town.UI
{
    [UIWindowDataAttribute(typeof(TownButtonsWin), false, "TownButtonsWin", 1)]
    public class TownButtonsWin : UI_WindowBase
    {
        [SerializeField] private Button _battleBtn;
        [SerializeField] private Button _departBtn;
        [SerializeField] private Button _buildBtn;

        public override void OnShow()
        {
            UISystem.Show<GoldWin>();
            _battleBtn.onClick.AddListener(OnBattleClick);
            _departBtn.onClick.AddListener(OnDepartClick);
            _buildBtn.onClick.AddListener(OnBuildClick);
        }

        public override void OnClose()
        {
            _battleBtn.onClick.RemoveAllListeners();
            _departBtn.onClick.RemoveAllListeners();
            _buildBtn.onClick.RemoveAllListeners();
        }

        private void OnBattleClick()
        {
            UISystem.Show<CombatEntriesWin>();
        }

        private void OnDepartClick()
        {
            UISystem.Close<TownButtonsWin>();
            SceneSystem.LoadScene("Level");
        }

        private void OnBuildClick()
        {
            UISystem.Show<TownBuildWin>();
        }
    }
}