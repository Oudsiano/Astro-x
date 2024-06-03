using DI;
using LevelObjects;
using Missions;
using PlayerProgress;
using Services;
using Services.DI;
using UnityEngine;

namespace Startup.InGame
{
    public class MissionsInitializer : InitializerBase
    {
        [SerializeField] private LevelObjectsStorage _levelObjectsStorage;
        [SerializeField] private ObjectsSpawnerService _objectsSpawnerService;

        [Inject] private GameInfoContainer _gameInfo;
        
        private LevelScoresCounter _levelScoresCounter;
        private MissionsController _missionsController;
        
        public override void Initialize()
        {
            GameContainer.InjectToInstance(this);
            GameContainer.InGame.Register(_levelObjectsStorage);
            GameContainer.InGame.Register(_objectsSpawnerService);
            
            _objectsSpawnerService.Initialize();
            
            _levelScoresCounter = GameContainer.Create<LevelScoresCounter>();
            GameContainer.InGame.Register(_levelScoresCounter);
            
            _missionsController = GameContainer.Create<MissionsController>();
            GameContainer.InGame.Register(_missionsController);

            _missionsController.StartMission(_gameInfo.CurrentLevel.LevelMission);
        }

        public override void Dispose()
        {
            if (_missionsController.MissionCompleted)
            {
                var progressManager = GameContainer.Common.Resolve<PlayerProgressManager>();
                progressManager.Data.AddMoney(Mathf.Max(0, _levelScoresCounter.CurrentScores));
                progressManager.SaveProgress();
            }
            
            _missionsController.Dispose();
        }

        public override void Reinitialize()
        {
            if (_missionsController.MissionCompleted)
            {
                var progressManager = GameContainer.Common.Resolve<PlayerProgressManager>();
                progressManager.Data.AddMoney(Mathf.Max(0, _levelScoresCounter.CurrentScores));
                progressManager.SaveProgress();
            }
            
            _levelScoresCounter.Reset();
            _missionsController.Reset();
        }
    }
}