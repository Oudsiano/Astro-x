using System;
using Damage;
using DI;
using LevelObjects;
using PlayerShips;
using Services.SoundsSystem;
using Services.WindowsSystem;
using Ui.Windows;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public event Action OnDestroyed;

        public PlayerShootController ShootController => _playerShoot;
        
        [SerializeField] private Damageable _playerDamageable;
        [SerializeField] private PlayerShootController _playerShoot;
        [SerializeField] private GameObject _engineFx;
        [SerializeField] private GameObject _shieldFx;

        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private SoundsSystem _soundsSystem;

        private bool _shieldActive;
        private PlayerShipContainer _shipContainer;

        public void Initialize(PlayerShipInfo shipInfo)
        {
            _shipContainer = Instantiate(shipInfo.ShipContainer, transform);
            _shipContainer.transform.MoveToLocalZero();
            _engineFx.transform.position = _shipContainer.EnginePoint.position;
            
            _playerShoot.Initialize(_shipContainer, shipInfo);
            _windowsSystem.TryGetWindow(out InGameUI window);
            
            _playerDamageable.Initialize(window.PlayerHealthStatus, shipInfo.MaxHp);
            _playerDamageable.OnDestroyed += OnPlayerDestroyed;
        }

        public void Dispose()
        {
            _playerDamageable.OnDestroyed -= OnPlayerDestroyed;

            if (_shipContainer == null) return;
            
            Destroy(_shipContainer.gameObject);
            _shipContainer = null;
        }

        public void SetShieldState(bool state)
        {
            _shieldActive = state;
            _shieldFx.SetActive(state);
            _playerDamageable.SetShield(state);
        }

        private void OnPlayerDestroyed(DamageType obj)
        {
            _soundsSystem.PlaySound(SoundType.PlayerDead);
            OnDestroyed?.Invoke();
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_shieldActive) return;
            
            var levelObject = other.collider.GetComponentInParent<LevelDestroyableObject>();
            if (levelObject == null) return;
            
            _playerDamageable.Damage(levelObject.Data.CollisionToPlayerDamagePercent * _playerDamageable.InitialHp, DamageType.Collision);
        }
    }
}