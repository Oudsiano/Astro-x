using System;
using Damage;
using DI;
using LevelObjects;
using LevelObjects.Messages;
using Player;
using PlayerProgress;
using Services;
using Services.DI;
using Services.WindowsSystem;
using Ui.Windows;
using UnityEngine;

namespace Missions
{
    [Serializable]
    public class MissionStage
    {
        private const float CheckObjectsInterval = 5f;
        
        [SerializeField] public LevelObjectType SpawnObjectsType;
        [SerializeField] public int ObjectsCount;
        [SerializeField] public LevelObjectData SpawnObjectData;
        [SerializeField] public float ShootCooldown;
        [SerializeField] public float Health;
        
        [Inject] private MessageBroker _messageBroker;
        [Inject] private MissionsController _missionsController;
        [Inject] private LevelObjectsStorage _levelObjectsStorage;
        [Inject] private ObjectsSpawnerService _spawnerService;
        [Inject] private LevelScoresCounter _scoresCounter;
        [Inject] private PlayerController _player;
        [Inject] private MonoUpdater _monoUpdater;
        [Inject] private WindowsSystem _windowsSystem;

        private bool _isCompleted;
        private int _destroyedCount;
        private int _displayDestroyedCount;
        private ObjectsSpawnerBase _spawner;
        private InGameUI _inGameUI;

        private float _checkObjectsTimer;
        
        public void Initialize(float speedMultiplier)
        {
            Debug.Log($"Initializing mission stage {SpawnObjectData.Type} with {ObjectsCount.ToString()} objects");
            
            _destroyedCount = 0;
            _displayDestroyedCount = -1;
            _isCompleted = false;
            
            GameContainer.InjectToInstance(this);
            _messageBroker.Subscribe<LevelObjectDestroyedMessage>(OnLevelObjectDestroyed);

            SpawnObjectsType = SpawnObjectData.Type;
            _spawner = _spawnerService.GetSpawnerForType(SpawnObjectsType);

            float shootCooldown = ShootCooldown > 0f ? ShootCooldown : SpawnObjectData.ShootCooldown;
            float health = Health > 0f ? Health : SpawnObjectData.Health;
            _spawner.StartSpawn(ObjectsCount, SpawnObjectData, shootCooldown, health, speedMultiplier);

            _monoUpdater.OnUpdate += OnUpdate;
        }

        private void OnUpdate()
        {
            _checkObjectsTimer += Time.deltaTime;
            if (_checkObjectsTimer > CheckObjectsInterval)
            {
                _checkObjectsTimer -= CheckObjectsInterval;
                _spawner.CheckObjects(ObjectsCount - _destroyedCount);
            }
            
            if (_inGameUI == null)
                _windowsSystem.TryGetWindow(out _inGameUI);

            if (_inGameUI != null && _displayDestroyedCount != _destroyedCount)
            {
                _displayDestroyedCount = _destroyedCount;
#if UNITY_EDITOR
                _inGameUI.SetStageText($"{SpawnObjectData.Type} - {_displayDestroyedCount.ToString()}/{ObjectsCount.ToString()}");
#endif
            }
            
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Y))
                _missionsController.CompleteStage(this);
#endif
            
            if (_isCompleted)
                _missionsController.CompleteStage(this);
        }

        private void OnLevelObjectDestroyed(ref LevelObjectDestroyedMessage message)
        {
            if (message.Data.Type != SpawnObjectsType)
                return;

            _destroyedCount++;
            Debug.Log($"Level object {SpawnObjectsType} destroyed! {_destroyedCount}/{ObjectsCount}");
            
            if (message.DamageType != DamageType.Collision)
                _scoresCounter.AddScores(SpawnObjectData.ScoresForDestroy);
            else
                _scoresCounter.TakeScores(SpawnObjectData.ScoresPenaltyOnCollision);
            
            if (_destroyedCount < ObjectsCount)
                return;

            _isCompleted = true;
        }

        public void Dispose()
        {
            Debug.Log($"Disposing mission stage {SpawnObjectsType} with {ObjectsCount.ToString()} objects");
            _messageBroker.Unsubscribe<LevelObjectDestroyedMessage>(OnLevelObjectDestroyed);
            _monoUpdater.OnUpdate -= OnUpdate;
            if (_spawner != null && _spawner.gameObject != null)
                _spawner.Dispose();
        }
    }
}