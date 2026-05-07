using TMPro;
using UnityEngine;

namespace Presentation.UI.Common
{
    public class EnergyUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _energyText;

        public void UpdateEnergy(int value)
        {
            if (_energyText != null)
            {
                _energyText.text = value.ToString();
            }
        }
    }
}
