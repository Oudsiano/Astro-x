using System.Globalization;
using DI;
using Levels;
using PlayerProgress;
using PlayerShips;
using Services;
using Services.DI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows.Extra
{
    public class ShipStoreTab : MonoBehaviour
    {
        [Header("Ship Info")]
        [SerializeField] private ShipPreviewPanel _preview;
        [SerializeField] private TextMeshProUGUI _shipNameText;
        [SerializeField] private TextMeshProUGUI _damageText;
        [SerializeField] private TextMeshProUGUI _fireRateText;
        [SerializeField] private TextMeshProUGUI _durabilityText;
        [SerializeField] private TextMeshProUGUI _damagePerSecondText;
        
        [Header("Ship Controls")]
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _prevButton;
        [SerializeField] private Button _buyButton;
        [SerializeField] private GameObject _canBuyIndicator;
        [SerializeField] private GameObject _cannotBuyIndicator;
        [SerializeField] private GameObject _alreadyBoughtIndicator;
        [SerializeField] private TextMeshProUGUI _buyCostText;
        [SerializeField] private ShipsStoreWindow _window;
        [SerializeField] private GameLevelsData _levelsData;

        [Inject] private PlayerProgressManager _progressManager;
        [Inject] private GameInfoContainer _gameInfoContainer;
        [Inject] private PlayerShipsData _playerShipsData;
        
        private int _currentViewingShip;

        private void Awake()
        {
            GameContainer.InjectToInstance(this);
        }

        private void Start()
        {
            _nextButton.onClick.AddListener(() =>
            {
                _currentViewingShip++;
                _currentViewingShip %= _playerShipsData.PlayerShips.Length;

                if (_playerShipsData.PlayerShips[_currentViewingShip].OpenOnAllLevelsCompleted)
                {
                    bool hasNotCompletedLevel = false;
                    foreach (var level in _levelsData.Levels)
                    {
                        hasNotCompletedLevel = !_progressManager.Data.CompletedLevels.Contains(level.Id);
                        if (hasNotCompletedLevel) break;
                    }

                    if (hasNotCompletedLevel)
                    {
                        _currentViewingShip++;
                        _currentViewingShip %= _playerShipsData.PlayerShips.Length;
                    }
                    else
                    {
                        Buy(true);
                    }
                }
                
                ShowShip();
            });
            
            _prevButton.onClick.AddListener(() =>
            {
                _currentViewingShip--;
                if (_currentViewingShip < 0) _currentViewingShip += _playerShipsData.PlayerShips.Length;
                
                if (_playerShipsData.PlayerShips[_currentViewingShip].OpenOnAllLevelsCompleted)
                {
                    bool hasNotCompletedLevel = false;
                    foreach (var level in _levelsData.Levels)
                    {
                        hasNotCompletedLevel = !_progressManager.Data.CompletedLevels.Contains(level.Id);
                        if (hasNotCompletedLevel) break;
                    }

                    if (hasNotCompletedLevel)
                    {
                        _currentViewingShip--;
                        if (_currentViewingShip < 0) _currentViewingShip += _playerShipsData.PlayerShips.Length;
                    }
                    else
                    {
                        Buy(true);
                    }
                }
                ShowShip();
            });
            
            _buyButton.onClick.AddListener(() =>
            {
                Buy();
                ShowShip();
            });
        }

        private void OnEnable()
        {
            ShowShip();
        }
        
        private void ShowShip()
        {
            var ship = _playerShipsData.PlayerShips[_currentViewingShip];
            _preview.SpawnShip(ship);

            float shotsPerSecond = Mathf.Round(1f / ship.ReloadTime * 100f) / 100f;
            float damagePerSecond = Mathf.Round(shotsPerSecond * ship.Damage * 100f) / 100f;
            
            _shipNameText.text = ship.Name;
            _damageText.text = $"Damage: {ship.Damage.ToString(CultureInfo.InvariantCulture)}";
            _fireRateText.text = $"Shots per second: {shotsPerSecond.ToString(CultureInfo.InvariantCulture)}";
            _durabilityText.text = $"Durability: {ship.MaxHp.ToString(CultureInfo.InvariantCulture)}";
            _damagePerSecondText.text = $"Damage per second: {damagePerSecond.ToString(CultureInfo.InvariantCulture)}";

            if (_progressManager.Data.BoughtShips.Contains(_currentViewingShip))
            {
                _buyButton.gameObject.SetActive(false);
                _cannotBuyIndicator.SetActive(false);
                _alreadyBoughtIndicator.gameObject.SetActive(true);
                return;
            }
            
            _alreadyBoughtIndicator.gameObject.SetActive(false);
            _buyButton.gameObject.SetActive(true);
            
            _buyCostText.text = ship.Cost.ToString();
            
            _canBuyIndicator.SetActive(ship.Cost <= _progressManager.Data.Money);
            _cannotBuyIndicator.SetActive(ship.Cost > _progressManager.Data.Money);
        }

        private void Buy(bool force = false)
        {
            var ship = _playerShipsData.PlayerShips[_currentViewingShip];
            if (ship.Cost > _progressManager.Data.Money && !force) return;

            _progressManager.Data.BoughtShips.Add(_currentViewingShip);
            if (!force)
                _progressManager.Data.AddMoney(-ship.Cost);
            
            _progressManager.SaveProgress();
            _window.UpdateMoney();
        }
    }
}