using Features.Dungeon.Domain;
using Features.Dungeon.Infrastructure;
using UnityEngine;

namespace Features.Dungeon.Application
{
    public class DungeonService : IDungeonService
    {
        private readonly IDungeonAlgorithm _algorithm;
        private readonly DungeonSettings _settings;
        private readonly DungeonCanvasSetup _canvasSetup;
        private readonly IRoomViewFactory _roomFactory;
        private readonly ILineViewFactory _lineFactory;

        public DungeonService(
            IDungeonAlgorithm algorithm,
            DungeonSettings settings,
            DungeonCanvasSetup canvasSetup,
            IRoomViewFactory roomFactory,
            ILineViewFactory lineFactory)
        {
            _algorithm = algorithm;
            _settings = settings;
            _canvasSetup = canvasSetup;
            _roomFactory = roomFactory;
            _lineFactory = lineFactory;
        }

        public void Generate()
        {
            Clear();
            _canvasSetup.EnsureCanvas();

            DungeonResult result = _algorithm.Generate(_settings);

            Vector2 gridOrigin = GetGridOrigin();

            foreach (RoomData roomData in result.Rooms)
            {
                Vector2 position = new(
                    gridOrigin.x + roomData.Position.X * _settings.Spacing,
                    gridOrigin.y + roomData.Position.Y * _settings.Spacing
                );

                RoomView roomView = _roomFactory.Create(roomData, position, _canvasSetup.RoomsContainer);
                roomView.Setup(roomData.Position.X, roomData.Position.Y, roomData.Type);
            }

            foreach (LineData lineData in result.Lines)
            {
                Vector2 fromPos = WorldToRoomCenter(lineData.From, gridOrigin);
                Vector2 toPos = WorldToRoomCenter(lineData.To, gridOrigin);
                _lineFactory.Create(lineData, fromPos, toPos, _canvasSetup.LinesContainer);
            }
        }

        public void Clear()
        {
            _canvasSetup.ClearGenerated();
        }

        private Vector2 GetGridOrigin()
        {
            float totalWidth = (_settings.GridWidth - 1) * _settings.Spacing;
            float totalHeight = (_settings.GridHeight - 1) * _settings.Spacing;
            return new Vector2(-totalWidth * 0.5f, -totalHeight * 0.5f);
        }

        private Vector2 WorldToRoomCenter(GridPosition pos, Vector2 gridOrigin)
        {
            return new Vector2(
                gridOrigin.x + pos.X * _settings.Spacing,
                gridOrigin.y + pos.Y * _settings.Spacing
            );
        }
    }
}