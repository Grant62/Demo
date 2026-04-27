using Features.Dungeon;
using Features.Dungeon.Domain;
using UnityEngine;

namespace Features.Dungeon.Application
{
    public interface IRoomViewFactory
    {
        RoomView Create(RoomData data, Vector2 position, RectTransform container);
    }
}
