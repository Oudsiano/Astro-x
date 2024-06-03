using System.Collections.Generic;
using System.Globalization;
using DI;
using PlayerProgress;
using PlayerShips;
using Services;
using Services.WindowsSystem;
using TMPro;
using Ui.Windows.Extra;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows
{
    public class ShipsHangarWindow : WindowBase
    {
        [Header("Common")]
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private ShipPreviewPanel _preview;
        
        [Header("Ship Controls")]
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _prevButton;
        [SerializeField] private Button _chooseButton;
        [SerializeField] private TextMeshProUGUI _chooseText;
        
        [Header("Ship Info")]
        [SerializeField] private TextMeshProUGUI _shipNameText;
        [SerializeField] private TextMeshProUGUI _damageText;
        [SerializeField] private TextMeshProUGUI _fireRateText;
        [SerializeField] private TextMeshProUGUI _durabilityText;

        [Inject] private PlayerProgressManager _progressManager;
        [Inject] private GameInfoContainer _gameInfoContainer;
        [Inject] private PlayerShipsData _playerShipsData;

        private int _currentViewingShip;
        private List<PlayerShipInfo> _playerShips;
        
        private void Start()
        {
            _moneyText.text = _progressManager.Data.Money.ToString();
            _preview.SpawnShip(_gameInfoContainer.CurrentShip);
            _shipNameText.text = _gameInfoContainer.CurrentShip.Name;
            _preview.Enter();

            _playerShips = new List<PlayerShipInfo>();
            foreach (var shipInfo in _playerShipsData.PlayerShips)
            {
                if (!_progressManager.Data.BoughtShips.Contains(shipInfo.Id))
                    continue;
                
                _playerShips.Add(shipInfo);
                if (shipInfo.Id == _progressManager.Data.SelectedShip)
                    _currentViewingShip = _playerShips.Count - 1;
            }
            
            _menuButton.onClick.AddListener(() =>
            {
                windowsSystem.DestroyWindow(this);
                windowsSystem.TryGetWindow<MainMenuWindow>(out var mainMenu);
                mainMenu.gameObject.SetActive(true);
            });
            
            _settingsButton.onClick.AddListener(() =>
            {
                windowsSystem.CreateWindow<SettingsWindow>();
            });
            
            _chooseButton.onClick.AddListener(() =>
            {
                _progressManager.Data.SelectedShip = _playerShips[_currentViewingShip].Id;
                _progressManager.SaveProgress();
                _gameInfoContainer.CurrentShip = _playerShips[_currentViewingShip];
                
                _chooseText.text = "Selected";
            });
            
            _nextButton.onClick.AddListener(() =>
            {
                _currentViewingShip++;
                _currentViewingShip %= _playerShips.Count;
                ShowShip(_currentViewingShip);
            });
            
            _prevButton.onClick.AddListener(() =>
            {
                _currentViewingShip--;
                if (_currentViewingShip < 0) _currentViewingShip += _playerShips.Count;
                ShowShip(_currentViewingShip);
            });
            
            ShowShip(_currentViewingShip);
        }

        private void ShowShip(int id)
        {
            _currentViewingShip = id;
            var ship = _playerShips[id];
            _preview.SpawnShip(ship);

            _shipNameText.text = ship.Name;
            _damageText.text = $"Damage: {ship.Damage.ToString(CultureInfo.InvariantCulture)}";
            _fireRateText.text = $"Shots per second: {(Mathf.Round(1f / ship.ReloadTime * 100f) / 100f).ToString(CultureInfo.InvariantCulture)}";
            _durabilityText.text = $"Durability: {ship.MaxHp.ToString(CultureInfo.InvariantCulture)}";

            _chooseText.text = _gameInfoContainer.CurrentShip == ship ? "Selected" : "Select this!";
        }
    }
}