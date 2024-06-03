using System;
using Cysharp.Threading.Tasks;
using DI;
using Services;
using Services.SoundsSystem;
using Ui;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Startup.GameStateMachine.GameStates
{
    public class PlayGameState : IGameState
    {
        private const string PlayScene = "PlayScene";
        private const string MainScene = "MainMenu";
        
        private const float PlanetShowMinTime = 0.5f;
        private const float PlanetShowMaxTime = 1f;
        
        [Inject] private SoundsSystem _soundsSystem;
        [Inject] private GameInfoContainer _gameInfoContainer;
        [Inject] private LoadingScreen _loadingScreen;

        public async UniTask OnEnter()
        {
            Debug.Log("Enter play state");
            _soundsSystem.PlayMusic(MusicType.LevelPreview);
            
            _loadingScreen.Active = true;
            _loadingScreen.SetLoadedState(false);
            _loadingScreen.OnPlayPressed = OnPlayPressed;
            _loadingScreen.UpdateFacts(_gameInfoContainer.CurrentLevel.Planet == null || !_gameInfoContainer.CurrentLevel.ShowPlanetInfo);
            
            await UniTask.Delay(Mathf.FloorToInt(Random.Range(PlanetShowMinTime, PlanetShowMaxTime) * 1000));

            try
            {
                var music = Resources.Load<AudioClip>(_gameInfoContainer.CurrentLevel.LevelMusicPath);
                _soundsSystem.PlayMusic(music);
            }
            catch (Exception)
            {
                Debug.Log($"Cannot load level music from path: {_gameInfoContainer.CurrentLevel.LevelMusicPath}");
            }
            
            Debug.Log("Load game scene");
            await SceneManager.LoadSceneAsync(PlayScene);
            
            _loadingScreen.SetLoadedState(true);
        }

        private void OnPlayPressed()
        {
            _loadingScreen.Active = false;
            var initializer = Object.FindObjectOfType<LevelInitializer>();
            initializer.Initialize();
        }

        public async UniTask OnExit()
        {
            Debug.Log("Exit play state");
            await SceneManager.LoadSceneAsync(MainScene);
        }
    }
}