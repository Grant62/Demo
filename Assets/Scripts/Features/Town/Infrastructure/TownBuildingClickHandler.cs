using Features.Town.Domain;
using UnityEngine;
using UnityEngine.EventSystems;
using EventSystem = JKFrame.EventSystem;

namespace Features.Town.Infrastructure
{
    [RequireComponent(typeof(Collider2D))]
    public class TownBuildingClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TownBuildingType _buildingType;

        public void OnPointerClick(PointerEventData eventData)
        {
            EventSystem.EventTrigger("TownBuildingClicked", _buildingType);
        }
    }
}