using Features.Dungeon.Application;
using Features.Dungeon.Domain;
using UnityEngine;

namespace Features.Dungeon.Infrastructure
{
    public class SupplyTrigger : MonoBehaviour, IDungeonInteractable
    {
        [SerializeField] private int _supplyId;

        private bool _interacted;

        public bool CanInteract => !_interacted;

        public void Interact()
        {
            if (_interacted)
                return;

            _interacted = true;
            SupplyService.DrawRewards(_supplyId);
        }
    }
}
