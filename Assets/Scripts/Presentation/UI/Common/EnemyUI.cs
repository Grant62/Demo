using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.UI.Common
{
    public class EnemyUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private Image _hpFillImage;
        [SerializeField] private Image _enemyImage;

        public void UpdateName(string name)
        {
            if (_nameText != null)
            {
                _nameText.text = name;
            }
        }

        public void UpdateHP(int currentHP, int maxHP)
        {
            if (_hpText != null)
            {
                _hpText.text = $"{currentHP}/{maxHP}";
            }

            if (_hpFillImage != null)
            {
                _hpFillImage.fillAmount = maxHP > 0 ? (float)currentHP / maxHP : 0f;
            }
        }

        public void UpdateEnemySprite(Sprite sprite)
        {
            if (_enemyImage != null)
            {
                _enemyImage.sprite = sprite;
            }
        }
    }
}
