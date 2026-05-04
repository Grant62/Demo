using Features.Town.Domain;
using UnityEngine;

namespace Features.Town.Infrastructure
{
    public class TownSceneObj : MonoBehaviour
    {
        [SerializeField] private GameObject _infantryCamp;
        [SerializeField] private GameObject _administrativeHall;
        [SerializeField] private GameObject _sentryPost;

        public void ActivateBuilding(TownBuildingType type)
        {
            GameObject target = GetBuildingObject(type);
            if (target != null)
            {
                target.SetActive(true);
            }
        }

        public bool IsBuildingActivated(TownBuildingType type)
        {
            GameObject target = GetBuildingObject(type);
            return target != null && target.activeSelf;
        }

        private GameObject GetBuildingObject(TownBuildingType type)
        {
            switch (type)
            {
                case TownBuildingType.战友团:
                    return _infantryCamp;
                case TownBuildingType.祖宅:
                    return _administrativeHall;
                case TownBuildingType.游侠箭阁:
                    return _sentryPost;
                default:
                    return null;
            }
        }
    }
}