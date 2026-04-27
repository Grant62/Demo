using Features.Dungeon.Application;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Features.Dungeon.Infrastructure
{
    public class DungeonEntryPoint : MonoBehaviour
    {
        private IDungeonService _dungeonService;

        [Inject]
        public void Construct(IDungeonService dungeonService)
        {
            _dungeonService = dungeonService;
        }

        private void Start()
        {
            Generate();
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            _dungeonService.Generate();
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            _dungeonService.Clear();
        }
    }
}
