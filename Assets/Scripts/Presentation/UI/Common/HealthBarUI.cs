using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.UI.Common
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private Image _hpFillImage;

        public void UpdateHp(int currentHp, int maxHp)
        {
            _hpText.text = currentHp <= 0 ? "死亡" : $"{currentHp}/{maxHp}";
            _hpFillImage.fillAmount = maxHp > 0 ? (float)currentHp / maxHp : 0f;
        }
    }
}