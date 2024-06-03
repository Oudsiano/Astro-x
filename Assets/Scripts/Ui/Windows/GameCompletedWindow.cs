using System.Linq;
using Cysharp.Threading.Tasks;
using DI;
using PlayerProgress;
using PlayerShips;
using Services.SoundsSystem;
using Services.WindowsSystem;
using Startup;
using Startup.GameStateMachine;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows
{
    public class GameCompletedWindow : WindowBase
    {
        [SerializeField] private Button _backButton;

        [Inject] private GameInitializer _gameInitializer;
        [Inject] private PlayerProgressManager _progressManager;
        [Inject] private PlayerShipsData _playerShipsData;
        [Inject] private SoundsSystem _soundsSystem;
        
        private void Start()
        {
            _progressManager.Data.IsGameCompleted = true;
            _progressManager.SaveProgress();
            
            _backButton.onClick.AddListener(() =>
            {
                windowsSystem.DestroyWindow(this);
                if (_gameInitializer.CurrentState == GameStateType.Play)
                {
                    _gameInitializer.BackToMenu().Forget();
                    _soundsSystem.PlaySound(SoundType.LevelExit);
                }
                else
                {
                    windowsSystem.TryGetWindow(out MainMenuWindow mainMenuWindow);
                    mainMenuWindow.gameObject.SetActive(true);
                }
            });

            var ship = _playerShipsData.PlayerShips.FirstOrDefault(x => x.OpenOnAllLevelsCompleted);
            if (ship == null) return;
            if (_progressManager.Data.BoughtShips.Contains(ship.Id)) return;
            
            _progressManager.Data.BoughtShips.Add(ship.Id);
            _progressManager.SaveProgress();
        }
    }
}