using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.UI.Common
{
    public class MagicPowerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _mpText;
        [SerializeField] private Image _mpFillImage;

        public void UpdateMp(int currentMp, int maxMp)
        {
            _mpText.text = currentMp <= 0 ? "耗尽" : $"{currentMp}/{maxMp}";
            _mpFillImage.fillAmount = maxMp > 0 ? (float)currentMp / maxMp : 0f;
        }
    }
}