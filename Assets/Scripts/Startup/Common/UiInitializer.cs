using Services.DI;
using Services.WindowsSystem;
using Ui;
using Ui.Windows;
using UnityEngine;

namespace Startup.Common
{
    public class UiInitializer : InitializerBase
    {
        [SerializeField] private UiRoot _uiRootPrefab;
        [SerializeField] private LoadingScreen _loadingScreenPrefab;
        [SerializeField] private GameWindows _gameWindows;
        
        public override void Initialize()
        {
            var uiRoot = Instantiate(_uiRootPrefab);
            GameContainer.Common.Register(uiRoot);
            GameContainer.Common.Register(_gameWindows);
            
            var windowsSystem = GameContainer.Create<WindowsSystem>();
            GameContainer.Common.Register(windowsSystem);

            var loadingScreen = GameContainer.InstantiateAndResolve(_loadingScreenPrefab, uiRoot.OverlayParent);
            loadingScreen.Active = false;
            GameContainer.Common.Register(loadingScreen);

            windowsSystem.CreateWindow<MainMenuWindow>();
            
            DontDestroyOnLoad(uiRoot);
        }
    }
}