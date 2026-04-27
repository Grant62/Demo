using Features.Dungeon;
using Features.Dungeon.Domain;
using UnityEngine;

namespace Features.Dungeon.Application
{
    public interface ILineViewFactory
    {
        LineView Create(LineData data, Vector2 fromPos, Vector2 toPos, RectTransform container);
    }
}
