using Cysharp.Threading.Tasks;
using DI;
using PlayerProgress;
using Services.SoundsSystem;
using Services.WindowsSystem;
using Ui.Windows;
using UnityEngine;

namespace Startup.GameStateMachine.GameStates
{
    public class MainMenuGameState : IGameState
    {
        [Inject] private SoundsSystem _soundsSystem;
        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private PlayerProgressManager _progressManager;
        
        public UniTask OnEnter()
        {
            Debug.Log("Enter menu state");
            
            if (_progressManager.Data.PlayedFirstLaunchMusic)
            {
                _soundsSystem.PlayMusic(MusicType.MainMenu);
            }
            else
            {
                _soundsSystem.PlayMusic(MusicType.FirstLaunch);
                _progressManager.Data.PlayedFirstLaunchMusic = true;
                _progressManager.SaveProgress();
            }
            
            _windowsSystem.TryGetWindow<MainMenuWindow>(out var mainMenu);
            mainMenu.gameObject.SetActive(true);
            
            return UniTask.CompletedTask;
        }

        public UniTask OnExit()
        {
            _windowsSystem.TryGetWindow<MainMenuWindow>(out var mainMenu);
            mainMenu.gameObject.SetActive(false);
            
            Debug.Log("Exit menu state");
            return UniTask.CompletedTask;
        }
    }
}