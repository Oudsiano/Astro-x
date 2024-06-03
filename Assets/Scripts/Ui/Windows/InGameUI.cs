using DI;
using Player.PowerUps;
using PlayerProgress;
using Services.Ads;
using Services.WindowsSystem;
using TMPro;
using Ui.Windows.Extra;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows
{
    public class InGameUI : WindowBase
    {
        public HealthStatus PlayerHealthStatus => _playerHealthStatus;
        
        [SerializeField] private Button _pauseButton;
        [SerializeField] private HealthStatus _playerHealthStatus;
        [SerializeField] private TextMeshProUGUI _scoresText;
        [SerializeField] private PowerUpButton _shieldButton;
        [SerializeField] private PowerUpButton _laserButton;
        [SerializeField] private TextMeshProUGUI _currentStageText;

        [Inject] private PlayerProgressManager _progressManager;
        [Inject] private PowerUpManager _powerUpManager;
        [Inject] private IAdsViewer _adsViewer;
         
        private void Start()
        {
            Application.focusChanged += OnFocusChanged;
            _pauseButton.onClick.AddListener(Pause);
            UpdateScoresValue(0);

            _shieldButton.OnClicked += ShieldClicked;
            _laserButton.OnClicked += LaserClicked;
            
            _shieldButton.SetCount(_progressManager.Data.GetPowerUpCount(PowerUpType.Shield));
            _laserButton.SetCount(_progressManager.Data.GetPowerUpCount(PowerUpType.Laser));
            
            _shieldButton.UpdateTimer(0);
            _laserButton.UpdateTimer(0);

            _currentStageText.text = "";
        }

        private void OnDestroy()
        {
            Application.focusChanged -= OnFocusChanged;
        }

        private void OnFocusChanged(bool focused)
        {
#if UNITY_EDITOR
            return;
#endif
            if (!_adsViewer.IsShowingAd)
                Pause();
        }
        
#if UNITY_EDITOR
        public void SetStageText(string text) => _currentStageText.text = text;
#endif

        public void UpdatePowerUpsCount()
        {
            _shieldButton.SetCount(_progressManager.Data.GetPowerUpCount(PowerUpType.Shield));
            _laserButton.SetCount(_progressManager.Data.GetPowerUpCount(PowerUpType.Laser));
        }

        private void ShieldClicked()
        {
            _powerUpManager.EnablePowerUp(PowerUpType.Shield);
            _shieldButton.SetCount(_progressManager.Data.GetPowerUpCount(PowerUpType.Shield));
        }

        private void LaserClicked()
        {
            _powerUpManager.EnablePowerUp(PowerUpType.Laser);
            _laserButton.SetCount(_progressManager.Data.GetPowerUpCount(PowerUpType.Laser));
        }

        public void UpdateScoresValue(int scores)
        {
            _scoresText.text = scores.ToString();
        }

        public void UpdatePowerUpTimer(PowerUpType type, int seconds)
        {
            var button = GetButton(type);
            button.UpdateTimer(seconds);
        }

        private PowerUpButton GetButton(PowerUpType type)
        {
            return type switch
            {
                PowerUpType.Shield => _shieldButton,
                PowerUpType.Laser => _laserButton,
                _ => _shieldButton
            };
        }

        private void Pause()
        {
            windowsSystem.CreateWindow<PauseWindow>();
            gameObject.SetActive(false);
        }
    }
}