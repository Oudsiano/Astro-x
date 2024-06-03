using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DI;
using Levels;
using Player.PowerUps;
using PlayerProgress;
using Services;
using Services.Ads;
using Services.SoundsSystem;
using Services.WindowsSystem;
using Startup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows
{
    public class MissionCompletedWindow : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _scoresText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private GameLevelsData _levelsData;
        [SerializeField] private GameObject _lastLevelIndicator;
        
        [Inject] private GameInitializer _gameInitializer;
        [Inject] private LevelScoresCounter _scoresCounter;
        [Inject] private SoundsSystem _soundsSystem;
        [Inject] private GameInfoContainer _gameInfoContainer;
        [Inject] private IAdsViewer _adsViewer;
        [Inject] private PlayerProgressManager _progressManager;

        private void Start()
        {
            _progressManager.Data.LastCompletedLevel = _gameInfoContainer.CurrentLevel.Id;
            
            _soundsSystem.PlaySound(SoundType.LevelComplete);
            _scoresText.text = _scoresCounter.CurrentScores.ToString();
            _restartButton.onClick.AddListener(Restart);
            _menuButton.onClick.AddListener(Menu);

            bool isLastLevel = _gameInfoContainer.CurrentLevel == _levelsData.Levels[^1];
            _continueButton.gameObject.SetActive(!isLastLevel);
            _lastLevelIndicator.SetActive(isLastLevel);

            if (isLastLevel) return;
            
            _continueButton.onClick.AddListener(Continue);
        }

        private void Restart()
        {
            windowsSystem.DestroyWindow(this);
            _gameInitializer.RestartCurrentGame();
        }

        private void Menu()
        {
            if (_gameInfoContainer.MissionsToShowAd > 0)
            {
                GoToMenu();
            }
            else
            {
                _adsViewer.CanShowInterstitial = true;
                _adsViewer.ShowInterstitial(_ =>
                {
                    ShowAd(true);
                });
                _gameInfoContainer.MissionsToShowAd = GameInfoContainer.MissionsBetweenAds;
            }
        }

        private void GoToMenu()
        {
            windowsSystem.DestroyWindow(this);
            bool isLastLevel = _gameInfoContainer.CurrentLevel == _levelsData.Levels[^1];
            if (isLastLevel)
            {
                windowsSystem.CreateWindow<GameCompletedWindow>();
            }
            else
            {
                _gameInitializer.BackToMenu().Forget();
                _soundsSystem.PlaySound(SoundType.LevelExit);
            }
        }

        private void Continue()
        {
            if (_gameInfoContainer.MissionsToShowAd > 0)
            {
                GoToNextLevel();
            }
            else
            {
                _adsViewer.CanShowInterstitial = true;
                _adsViewer.ShowInterstitial(_ =>
                {
                    ShowAd(false);
                });
                _gameInfoContainer.MissionsToShowAd = GameInfoContainer.MissionsBetweenAds;
            }
        }

        private void GoToNextLevel()
        {
            int levelId = _gameInfoContainer.CurrentLevel.Id + 1;
            if (levelId < 0 || levelId > _levelsData.Levels.Length - 1)
                levelId = _levelsData.Levels.Length - 1;
                    
            var levelInfo = _levelsData.Levels[levelId];
            windowsSystem.DestroyWindow(this);
            _gameInfoContainer.CurrentLevel = levelInfo;

            if (levelInfo.ShowPlanetInfo && levelInfo.Planet != null && levelInfo.Planet.InfoSlides is { Count: > 0 })
            {
                var slidesWindow = windowsSystem.CreateWindow<PlanetInfoSlidesWindow>();
                slidesWindow.SetLevel(levelInfo);
            }
            else
            {
                _gameInitializer.GoToMenuAndStartGame().Forget();
            }
        }

        private void ShowAd(bool toMenu)
        {
            Debug.Log("Interstitial complete");
            int count = _progressManager.Data.BoughtPowerUps.GetValueOrDefault(PowerUpType.Shield, 0);
            _progressManager.Data.BoughtPowerUps[PowerUpType.Shield] = count + 1;
            _progressManager.SaveProgress();

            if (windowsSystem.TryGetWindow(out PauseWindow pauseWindow))
                pauseWindow.Close();
            
            if (windowsSystem.TryGetWindow(out InGameUI inGameUI))
                inGameUI.UpdatePowerUpsCount();
            
            if (toMenu) GoToMenu();
            else GoToNextLevel();
        }
    }
}