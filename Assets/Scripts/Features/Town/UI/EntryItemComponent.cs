using Features.Town.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Town.UI
{
    public class EntryItemComponent : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;

        private RecruitEntry _recruitEntry;

        public void Init(RecruitEntry entry)
        {
            _recruitEntry = entry;
        }

        public RecruitEntry GetEntry()
        {
            return _recruitEntry;
        }
    }
}