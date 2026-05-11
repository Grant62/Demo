using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Features.Action.UI
{
    public class BarWidget : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TMP_Text _amountText;

        public void UpdateValue(int current, int max)
        {
            _fillImage.fillAmount = (float)current / max;
            _amountText.text = $"{current}/{max}";
        }
    }
}
