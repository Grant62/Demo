namespace Features.Dungeon.Domain
{
    public interface IDungeonInteractable
    {
        bool CanInteract { get; }
        void Interact();
    }
}
