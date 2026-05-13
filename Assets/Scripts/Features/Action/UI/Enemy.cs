using Configuration.ExcelData.DataClass;
using TMPro;
using UnityEngine;

namespace Features.Action.UI
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private BarWidget _hpBar;

        public void Setup(EnemyInfo info, int atk, int hp)
        {
            _infoText.text = $"{info.Name}\n攻击力：{atk}";
            _hpBar.UpdateValue(hp, hp);
        }

        public void UpdateHP(int current, int max)
        {
            _hpBar.UpdateValue(current, max);
        }
    }
}