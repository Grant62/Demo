using Features.Town.Application;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Town.UI
{
    public class BuildItemComponent : MonoBehaviour
    {
        [SerializeField] private Button _buildBtn;
        [SerializeField] private TMP_Text _nameCostText;
        [SerializeField] private TMP_Text _spaceCostText;

        public void Init(BuildingConfig config, System.Action onClick)
        {
            _nameCostText.text = $"{config.displayName}  {config.goldCost}金";
            _spaceCostText.text = $"{config.spaceCost}费";
            _buildBtn.onClick.RemoveAllListeners();
            _buildBtn.onClick.AddListener(() => onClick?.Invoke());
        }

        public void SetInteractable(bool interactable)
        {
            _buildBtn.interactable = interactable;
        }
    }
}