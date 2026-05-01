using Features.Town.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Town.UI
{
    public class ResultItemComponent : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descText;

        private RecruitEntry _recruitEntry;

        public void Init(RecruitEntry entry)
        {
            _recruitEntry = entry;
            _nameText.text = entry.displayName;
            _descText.text = entry.description;
        }

        public RecruitEntry GetEntry()
        {
            return _recruitEntry;
        }
    }
}