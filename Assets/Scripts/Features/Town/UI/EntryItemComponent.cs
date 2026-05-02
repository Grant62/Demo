using Configuration.ExcelData.DataClass;
using JKFrame;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Town.UI
{
    public class EntryItemComponent : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;

        private OccupationInfo _occupationInfo;

        public void Init(OccupationInfo info)
        {
            _occupationInfo = info;

            if (_iconImage != null && !string.IsNullOrEmpty(info.ResAddress))
            {
                _iconImage.sprite = ResSystem.LoadAsset<Sprite>(info.ResAddress);
            }
        }

        public OccupationInfo GetOccupation()
        {
            return _occupationInfo;
        }
    }
}
