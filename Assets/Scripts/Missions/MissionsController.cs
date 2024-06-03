using System.Collections.Generic;
using DI;
using Player.PowerUps;
using PlayerProgress;
using Services;
using Services.Ads;
using Services.SoundsSystem;
using Services.WindowsSystem;
using Ui.Windows;
using UnityEngine;

namespace Missions
{
    public class MissionsController
    {
        public bool MissionCompleted { get; private set; }
        
        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private GameInfoContainer _gameInfoContainer;
        [Inject] private PlayerProgressManager _progressManager;
        [Inject] private LevelScoresCounter _scoresCounter;
        [Inject] private SoundsSystem _soundsSystem;
        [Inject] private IAdsViewer _adsRunner;
        
        private MissionData _data;
        private MissionStage _currentStage;
        private Queue<MissionStage> _stages;
        
        public void StartMission(MissionData data)
        {
            Debug.Log($"Starting mission with {data.Stages.Length} stages!");
            MissionCompleted = false;
            _data = data;

            _adsRunner.CanShowInterstitial = false;

            _stages = new Queue<MissionStage>();
            foreach (var stage in data.Stages)
            {
                _stages.Enqueue(stage);
            }
            
            MoveToNextStage();
        }
        
        public void CompleteStage(MissionStage stage)
        {
            Debug.Log("Complete stage");
            if (stage == null)
            {
                Debug.LogError("Trying to complete null stage");
                return;
            }
            if (_currentStage == null)
            {
                stage.Dispose();
                Debug.LogError("Trying to complete stage, but current not started");
                return;
            }
            if (stage != _currentStage)
            {
                stage.Dispose();
                Debug.LogError("Trying to complete not current stage");
                return;
            }
            
            Debug.Log("Dispose stage");
            Dispose();
            
            MoveToNextStage();
            _soundsSystem.PlaySound(SoundType.MissionComplete);
        }

        public void Dispose()
        {
            _currentStage?.Dispose();
            _currentStage = null;
        }

        public void Reset()
        {
            Dispose();
            StartMission(_data);
        }
        
        private void MoveToNextStage()
        {
            Debug.Log("Move to next stage");
            if (_currentStage != null)
            {
                Debug.LogError("Trying to start next mission stage, while prev not finished!");
                return;
            }
            
            if (!_stages.TryDequeue(out var stage))
            {
                Debug.Log("Cannot get new stage");
                CompleteMission();
                return;
            }

            Debug.Log("Initialize new stage!");
            _currentStage = stage;
            _currentStage.Initialize(_data.SpeedMultiplier);
        }

        private void CompleteMission()
        {
            Debug.Log("Completing mission");
            MissionCompleted = true;
            
            if (!_progressManager.Data.CompletedLevels.Contains(_gameInfoContainer.CurrentLevel.Id))
            {
                _progressManager.Data.CompletedLevels.Add(_gameInfoContainer.CurrentLevel.Id);
                _progressManager.SaveProgress();
            }
            
            _scoresCounter.AddScores(_data.RewardScoresForFullMission);
            
            _gameInfoContainer.MissionsToShowAd--;
            _windowsSystem.CreateWindow<MissionCompletedWindow>();
            // if (_gameInfoContainer.MissionsToShowAd > 0)
            // {
            //     _windowsSystem.CreateWindow<MissionCompletedWindow>();
            // }
            // else
            // {
            //     _adsRunner.CanShowInterstitial = true;
            //     _adsRunner.ShowInterstitial(_ =>
            //     {
            //         Debug.Log("Interstitial complete");
            //         int count = _progressManager.Data.BoughtPowerUps.GetValueOrDefault(PowerUpType.Shield, 0);
            //         _progressManager.Data.BoughtPowerUps[PowerUpType.Shield] = count + 1;
            //         _progressManager.SaveProgress();
            //
            //         if (_windowsSystem.TryGetWindow(out PauseWindow pauseWindow))
            //             pauseWindow.Close();
            //         
            //         _windowsSystem.CreateWindow<MissionCompletedWindow>();
            //         if (_windowsSystem.TryGetWindow(out InGameUI inGameUI))
            //             inGameUI.UpdatePowerUpsCount();
            //         
            //     });
            //     _gameInfoContainer.MissionsToShowAd = GameInfoContainer.MissionsBetweenAds;
            // }
        }
    }
}