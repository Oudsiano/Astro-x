using GameSettings;
using Levels;
using Player.PowerUps;
using PlayerProgress;
using PlayerShips;
using Services;
using Services.Ads;
using Services.DI;
using Services.SoundsSystem;
using UnityEngine;

namespace Startup.Common
{
    public class ServicesInitializer : InitializerBase
    {
        [SerializeField] private SoundsSystem _soundsSystemPrefab;
        [SerializeField] private PlayerShipsData _playerShipsData;
        [SerializeField] private GameLevelsData _levelsData;
        [SerializeField] private ShopPowerUpsTabData _powerUpsTabData;
        [SerializeField] private SpaceFactsInfo _spaceFactsInfo;
        
        public override void Initialize()
        {
            _levelsData.Initialize();
            
            var messageBroker = new MessageBroker();
            GameContainer.Common.Register(messageBroker);

            var gameSettings = new GameSettingsManager();
            GameContainer.Common.Register(gameSettings);

            var soundsSystem = GameContainer.InstantiateAndResolve(_soundsSystemPrefab);
            GameContainer.Common.Register(soundsSystem);

            var monoUpdaterGO = new GameObject("Mono Updater");
            var monoUpdater = monoUpdaterGO.AddComponent<MonoUpdater>();
            GameContainer.Common.Register(monoUpdater);

            var gameInfoContainer = GameContainer.Create<GameInfoContainer>();
            gameInfoContainer.LastAdReceivedRewardTime = float.MinValue;
            gameInfoContainer.MissionsToShowAd = GameInfoContainer.MissionsBetweenAds;
            GameContainer.Common.Register(gameInfoContainer);

            var progressManager = GameContainer.Create<PlayerProgressManager>();
            progressManager.LoadProgress();
            GameContainer.Common.Register(progressManager);

            if (!progressManager.Data.IsGameCompleted)
            {
                bool allLevelsCompleted = true;
                foreach (var levelInfo in _levelsData.Levels)
                {
                    if (!progressManager.Data.CompletedLevels.Contains(levelInfo.Id))
                        allLevelsCompleted = false;
                }

                if (allLevelsCompleted)
                {
                    progressManager.Data.IsGameCompleted = true;
                    progressManager.SaveProgress();
                }
            }

            _playerShipsData.Initialize();
            
            if (!progressManager.Data.BoughtShips.Contains(progressManager.Data.SelectedShip) || progressManager.Data.SelectedShip >= _playerShipsData.PlayerShips.Length)
                progressManager.Data.SelectedShip = 0;

            gameInfoContainer.CurrentShip = _playerShipsData.PlayerShips[progressManager.Data.SelectedShip];
            GameContainer.Common.Register(_playerShipsData);

            GameContainer.Common.Register(_powerUpsTabData);
            GameContainer.Common.Register(_spaceFactsInfo);

#if UNITY_EDITOR
            var adsViewerGo = new GameObject("Unity Ads Viewer");
            var adsViewer = adsViewerGo.AddComponent<UnityAdsViewer>();
            adsViewer.Initialize(); 
#elif UNITY_ANDROID
            var adsViewerGo = new GameObject("Yandex Ads Viewer");
            var adsViewer = adsViewerGo.AddComponent<YandexAdsViewer>();
            adsViewer.Initialize();
#endif
            
            GameContainer.Common.Register<IAdsViewer>(adsViewer);
            
            DontDestroyOnLoad(soundsSystem);
            DontDestroyOnLoad(monoUpdater);
            DontDestroyOnLoad(adsViewer);
        }
    }
}