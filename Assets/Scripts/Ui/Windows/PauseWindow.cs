using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DI;
using Player.PowerUps;
using PlayerProgress;
using Services;
using Services.Ads;
using Services.SoundsSystem;
using Services.WindowsSystem;
using Startup;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows
{
    public class PauseWindow : WindowBase
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _showAdButton;

        [SerializeField] private Image _powerUpImage;
        [SerializeField] private Color _inactivePowerUpColor;
        [SerializeField] private GameObject _timerIcon;

        [Inject] private GameInitializer _gameInitializer;
        [Inject] private SoundsSystem _soundsSystem;
        [Inject] private IAdsViewer _adsRunner;
        [Inject] private PlayerProgressManager _progressManager;
        [Inject] private GameInfoContainer _gameInfoContainer;

        private void Start()
        {
            Time.timeScale = 0f;
            _adsRunner.CanShowRewarded = true;
            
            _continueButton.onClick.AddListener(Close);
            _settingsButton.onClick.AddListener(OpenSettings);
            _menuButton.onClick.AddListener(GoToMenu);
            _restartButton.onClick.AddListener(Restart);
            _showAdButton.onClick.AddListener(ShowAd);
            
            UpdatePowerUpIcon();
        }

        private void OnDestroy()
        {
            _adsRunner.CanShowRewarded = false;
        }

        public void Close()
        {
            Time.timeScale = 1f;
            windowsSystem.DestroyWindow(this);
            windowsSystem.TryGetWindow(out InGameUI inGameUI);
            inGameUI.gameObject.SetActive(true);
        }

        private void OpenSettings()
        {
            windowsSystem.CreateWindow<SettingsWindow>();
        }

        private void GoToMenu()
        {
            Close();
            _gameInitializer.BackToMenu().Forget();
            _soundsSystem.PlaySound(SoundType.LevelExit);
        }

        private void Restart()
        {
            Close();
            _gameInitializer.RestartCurrentGame();
        }

        private void ShowAd()
        {
            _adsRunner.ShowRewarded(completed =>
            {
                Debug.Log($"Rewarded ad result: {completed}");
                if (!completed) return;
                
                if (_gameInfoContainer.CanGivePowerUp)
                {
                    int count = _progressManager.Data.BoughtPowerUps.GetValueOrDefault(PowerUpType.Laser, 0);
                    _progressManager.Data.BoughtPowerUps[PowerUpType.Laser] = count + 1;
                    _progressManager.SaveProgress();
                    _gameInfoContainer.LastAdReceivedRewardTime = Time.time;
                }
                
                UpdatePowerUpIcon();
                
                if (windowsSystem.TryGetWindow(out InGameUI inGameUI))
                    inGameUI.UpdatePowerUpsCount();
            });
        }

        private void UpdatePowerUpIcon()
        {
            bool canGivePowerUp = _gameInfoContainer.CanGivePowerUp;
            
            Debug.Log($"Check power up for ad timer: passed time: {Time.time - _gameInfoContainer.LastAdReceivedRewardTime}, can give: {canGivePowerUp}");

            _powerUpImage.color = canGivePowerUp ? Color.white : _inactivePowerUpColor;
            _timerIcon.SetActive(!canGivePowerUp);
        }
    }
}