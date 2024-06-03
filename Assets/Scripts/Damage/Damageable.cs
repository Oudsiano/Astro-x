using System;
using Ui.Windows;
using UnityEngine;

namespace Damage
{
    public class Damageable : MonoBehaviour
    {
        public event Action<DamageType> OnDestroyed;
        public float InitialHp => _initialHealth;
    
        [Header("Health")]
        [SerializeField] private float _initialHealth;
    
        [Header("Shield")]
        [SerializeField] private GameObject _shieldFx;

        [Header("VFX")]
        [SerializeField] private GameObject _destroyVfx;
    
        private HealthStatus _healthStatus;

        private bool _shieldIsActive;
        private float _currentHealth;

        public void Initialize(HealthStatus healthStatus, float initialHealth = -1)
        {
            _healthStatus = healthStatus;
            
            float hp = initialHealth > 0 ? initialHealth : _initialHealth;
            _currentHealth = hp;
            _initialHealth = hp;
            
            _healthStatus.HealthBarChange(hp, hp);

            if (_shieldFx != null) _shieldFx.SetActive(_shieldIsActive);
        }

        public void SetShield(bool state) => _shieldIsActive = state;

        public void Damage(float damage, DamageType damageType)
        {
            if (_shieldIsActive) return;
            
            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _initialHealth);
            try
            {
                _healthStatus.HealthBarChange(_currentHealth, _initialHealth);
            }
            catch (Exception)
            {
                // ignored
            }

            if (_currentHealth <= 0)
                Destruct(damageType);
            
            // if (_shieldFx != null)
            //     _shieldFx.SetActive(_shieldIsActive);
        }

        public void Destruct(DamageType damageType)
        {
            Instantiate(_destroyVfx, transform.position, transform.rotation);
            OnDestroyed?.Invoke(damageType);
            Destroy(gameObject);
        }
    }
}
