using DI;
using Services.DI;
using Services.WindowsSystem;
using Ui.Windows;
using UnityEngine;

namespace Player.PowerUps.Behaviours
{
    public class PowerUpLaserBehaviour : MonoBehaviour
    {
        [Inject] private PlayerController _player;
        [Inject] private WindowsSystem _windowsSystem;

        private float _timer;
        private InGameUI _inGameUI;

        public void Enable(PowerUpData data)
        {
            GameContainer.InjectToInstance(this);
            _timer += data.ActiveTime;
            _player.ShootController.BoostDamage(3f);
        }

        public void ResetTimers()
        {
            _timer = 0f;
        }

        private void Update()
        {
            if (_timer <= 0f)
            {
                if (_player != null)
                    _player.ShootController.DisableBoost();
                
                return;
            }

            _timer -= Time.deltaTime;

            if (!_windowsSystem.TryGetWindow(out _inGameUI))
                return;
            
            int seconds = Mathf.CeilToInt(_timer);
            _inGameUI.UpdatePowerUpTimer(PowerUpType.Laser, seconds);
            
            if (_timer > 0f) return;
            
            _player.ShootController.DisableBoost();
            _inGameUI.UpdatePowerUpTimer(PowerUpType.Laser, 0);
        }
    }
}