using Features.Dungeon.Application;
using Features.Dungeon.Domain;
using Features.Dungeon.Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Features.Dungeon.DI
{
    public class DungeonLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameObject _roomPrefab;
        [SerializeField] private GameObject _linePrefab;
        [SerializeField] private DungeonSettings _settings;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IDungeonAlgorithm, DungeonAlgorithm>(Lifetime.Singleton);
            builder.RegisterInstance(_settings);

            builder.Register<DungeonCanvasSetup>(Lifetime.Singleton);

            builder.Register<IRoomViewFactory, RoomViewFactory>(Lifetime.Transient)
                .WithParameter(typeof(GameObject), _roomPrefab);
            builder.Register<ILineViewFactory, LineViewFactory>(Lifetime.Transient)
                .WithParameter(typeof(GameObject), _linePrefab);

            builder.Register<IDungeonService, DungeonService>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<DungeonEntryPoint>();
        }
    }
}
