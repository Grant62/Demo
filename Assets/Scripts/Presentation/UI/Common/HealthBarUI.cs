using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.UI.Common
{
    public class HealthBarUI : MonoBehaviour
    {
        public GameObject healthBarObj;
        public GameObject bossHealthBarObj;
        private GameObject _curHealthBarObj;
        private Image _bar;
        private TMP_Text _healthText;

        public void Init(bool isBoss = false)
        {
            if (_curHealthBarObj != null)
                Destroy(_curHealthBarObj);

            _curHealthBarObj = Instantiate(isBoss ? bossHealthBarObj : healthBarObj, transform);
            _bar = _curHealthBarObj.transform.Find("HealthBar").GetComponentInChildren<Image>();
            _healthText = _curHealthBarObj.GetComponentInChildren<TMP_Text>();
        }

        public void UpdateHealthBar(float health, float maxHealth)
        {
            _bar.DOKill();
            _bar.DOFillAmount(health / maxHealth, 0.2f).SetEase(Ease.Linear);
            UpdateHealthText((int)health);
        }

        public void UpdateHealthText(int currentHealth)
        {
            if (currentHealth <= 0)
                _healthText.text = "死亡";
            else
                _healthText.text = "HP:" + currentHealth;
        }
    }
}
