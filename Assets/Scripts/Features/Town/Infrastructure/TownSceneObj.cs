using Features.Town.Domain;
using UnityEngine;

namespace Features.Town.Infrastructure
{
    public class TownSceneObj : MonoBehaviour
    {
        [SerializeField] private GameObject _infantryCamp;
        [SerializeField] private GameObject _administrativeHall;
        [SerializeField] private GameObject _church;
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
                case TownBuildingType.步兵营:
                    return _infantryCamp;
                case TownBuildingType.行政大厅:
                    return _administrativeHall;
                case TownBuildingType.教堂:
                    return _church;
                case TownBuildingType.哨兵所:
                    return _sentryPost;
                default:
                    return null;
            }
        }
    }
}