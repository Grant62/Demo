using Configuration.ExcelData.DataClass;
using JKFrame;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Town.UI
{
    public class ResultItemComponent : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;

        public void Init(OccupationInfo info)
        {
            if (_iconImage != null && !string.IsNullOrEmpty(info.ResAddress))
            {
                _iconImage.sprite = ResSystem.LoadAsset<Sprite>(info.ResAddress);
            }
        }
    }
}
