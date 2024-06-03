using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows
{
    public class HealthStatus : MonoBehaviour
    {
        [SerializeField] private Image _healthBar;

        public void HealthBarChange(float currentHealth, float maxHealth)
        {
            if (_healthBar != null)
                _healthBar.fillAmount = currentHealth / maxHealth;
        }
    }
}
