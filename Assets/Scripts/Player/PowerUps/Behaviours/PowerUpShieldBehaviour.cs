using DI;
using Services.DI;
using Services.WindowsSystem;
using Ui.Windows;
using UnityEngine;

namespace Player.PowerUps.Behaviours
{
    public class PowerUpShieldBehaviour : MonoBehaviour
    {
        [Inject] private PlayerController _player;
        [Inject] private WindowsSystem _windowsSystem;
        
        private float _timer;
        private InGameUI _inGameUI;
        
        public void Enable(PowerUpData data)
        {
            GameContainer.InjectToInstance(this);
            _windowsSystem.TryGetWindow(out _inGameUI);
            _timer += data.ActiveTime;
            _player.SetShieldState(true);
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
                    _player.SetShieldState(false);
                
                return;
            }

            _timer -= Time.deltaTime;
            int seconds = Mathf.CeilToInt(_timer);
            _inGameUI.UpdatePowerUpTimer(PowerUpType.Shield, seconds);
            
            if (_timer > 0f) return;
            
            _player.SetShieldState(false);
            _inGameUI.UpdatePowerUpTimer(PowerUpType.Shield, 0);
        }
    }
}