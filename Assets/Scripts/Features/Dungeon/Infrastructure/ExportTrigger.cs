using Features.Dungeon.Application;
using Features.Dungeon.Domain;
using UnityEngine;

namespace Features.Dungeon.Infrastructure
{
    public class ExportTrigger : MonoBehaviour, IDungeonInteractable
    {
        public bool CanInteract => !DungeonService.IsGameOver;

        public void Interact()
        {
            DungeonService.OnPlayerReachedExport();
        }
    }
}
