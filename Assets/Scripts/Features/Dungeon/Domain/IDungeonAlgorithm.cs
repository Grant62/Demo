namespace Features.Dungeon.Domain
{
    public interface IDungeonAlgorithm
    {
        DungeonResult Generate(DungeonSettings settings);
    }
}