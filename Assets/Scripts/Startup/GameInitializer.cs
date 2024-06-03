using Cysharp.Threading.Tasks;
using Services.DI;
using Startup.GameStateMachine;
using UnityEngine;
using GameContainer = Services.DI.GameContainer;

namespace Startup
{
    public class GameInitializer : MonoBehaviour
    {
        private static bool _initialized;

        public GameStateType CurrentState => _gameStateMachine.CurrentStateType;
        
        [SerializeField] private InitializerBase[] _commonInitializers;
        
        private GameStateMachine.GameStateMachine _gameStateMachine;
        
        private void Awake()
        {
            if (_initialized)
            {
                DestroyImmediate(gameObject);
                return;
            }

            _initialized = true;
            DontDestroyOnLoad(this);
            Initialize().Forget();
        }

        public void RestartCurrentGame()
        {
            Debug.Log("Restarting current level");
            var levelInitializer = GameContainer.InGame.Resolve<LevelInitializer>();
            levelInitializer.Reinitialize();
        }

        public async UniTask StartGame()
        {
            Debug.Log("Starting game");
            await _gameStateMachine.SwitchToState(GameStateType.Play);
        }
        
        public async UniTask BackToMenu()
        {
            Debug.Log("Return back to menu");
            await _gameStateMachine.SwitchToState(GameStateType.MainMenu);
            GameContainer.InGame?.Dispose();
            GameContainer.InGame = null;
        }

        public async UniTask GoToMenuAndStartGame()
        {
            await BackToMenu();
            await StartGame();
        }

        private async UniTask Initialize()
        {
            Debug.Log("Start initialize");
            GameContainer.Common = new Container();
            GameContainer.Common.Register(this);
            
            foreach (var initializer in _commonInitializers)
            {
                Debug.Log($"Initializing {initializer.GetType().Name}");
                initializer.Initialize();
            }
            
            Debug.Log("Creating state machine");
            _gameStateMachine = new GameStateMachine.GameStateMachine();
            
            Debug.Log("Switching to state main menu");
            await _gameStateMachine.SwitchToState(GameStateType.MainMenu, force: true);
        }
    }
}