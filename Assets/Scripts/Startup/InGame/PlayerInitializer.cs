using DI;
using LevelObjects;
using Player;
using Services;
using Services.DI;
using Services.ObjectPools;
using Services.WindowsSystem;
using Ui.Windows;
using UnityEngine;

namespace Startup.InGame
{
    public class PlayerInitializer : InitializerBase
    {
        [SerializeField] private Transform _playerSpawn;
        [SerializeField] private PlayerController _playerPrefab;

        [Inject] private GameInfoContainer _gameInfoContainer;
        [Inject] private WindowsSystem _windowsSystem;

        private PlayerController _player;
        
        public override void Initialize()
        {
            GameContainer.InjectToInstance(this);
            
            _player = GameContainer.InstantiateAndResolve(_playerPrefab);
            _player.Initialize(_gameInfoContainer.CurrentShip);
            
            GameContainer.InGame.Register(_player);
            
            _player.transform.position = _playerSpawn.position;
            _player.OnDestroyed += OnPlayerDestroyed;
        }

        private void OnPlayerDestroyed()
        {
            _windowsSystem.CreateWindow<MissionFailedWindow>();
            _player.OnDestroyed -= OnPlayerDestroyed;
            _player.Dispose();
            Destroy(_player.gameObject);
            _player = null;
        }

        public override void Dispose()
        {
            if (_player != null)
                _player.OnDestroyed -= OnPlayerDestroyed;
        }

        public override void Reinitialize()
        {
            if (_player != null)
            {
                _player.OnDestroyed -= OnPlayerDestroyed;
                _player.Dispose();
                Destroy(_player.gameObject);
            }
            
            _player = GameContainer.InstantiateAndResolve(_playerPrefab);
            _player.Initialize(_gameInfoContainer.CurrentShip);
            
            GameContainer.InGame.Register(_player, true);
            
            _player.transform.position = _playerSpawn.position;
            _player.OnDestroyed += OnPlayerDestroyed;
            
            var collectables = FindObjectsByType<Collectable>(FindObjectsSortMode.None);
            foreach (var collectable in collectables)
            {
                PrefabMonoPool<Collectable>.ReturnInstance(collectable);
            }
        }
    }
}