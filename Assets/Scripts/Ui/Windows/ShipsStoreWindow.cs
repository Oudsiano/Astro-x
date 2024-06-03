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
    public class ShipsStoreWindow : WindowBase
    {
        [Header("Common")]
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _shipsTabButton;
        [SerializeField] private Button _powerUpsTabButton;
        [SerializeField] private ShipStoreTab _shipStoreTab;
        [SerializeField] private PowerUpsStoreTab _powerUpsStoreTab;
        
        [Inject] private PlayerProgressManager _progressManager;
        [Inject] private GameInfoContainer _gameInfoContainer;
        [Inject] private PlayerShipsData _playerShipsData;
        
        private float _touchTimer;
        
        private void Start()
        {
            _moneyText.text = _progressManager.Data.Money.ToString();
            
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
            
            _shipsTabButton.onClick.AddListener(() =>
            {
                _shipStoreTab.gameObject.SetActive(true);
                _powerUpsStoreTab.gameObject.SetActive(false);
            });
            
            _powerUpsTabButton.onClick.AddListener(() =>
            {
                _shipStoreTab.gameObject.SetActive(false);
                _powerUpsStoreTab.gameObject.SetActive(true);
            });
            
            _shipStoreTab.gameObject.SetActive(true);
            _powerUpsStoreTab.gameObject.SetActive(false);
        }

        public void UpdateMoney()
        {
            _moneyText.text = _progressManager.Data.Money.ToString();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                GiveMoney();

            if (Input.touchCount > 0)
            {
                _touchTimer += Time.deltaTime;
                if (_touchTimer > 3f)
                {
                    GiveMoney();
                    _touchTimer = 0f;
                }
            }
            else
            {
                _touchTimer = 0f;
            }
        }

        private void GiveMoney()
        {
            _progressManager.Data.AddMoney(10000);
            _progressManager.SaveProgress();
            _moneyText.text = _progressManager.Data.Money.ToString();
            
            _shipStoreTab.gameObject.SetActive(_shipStoreTab.gameObject.activeSelf);
            _powerUpsStoreTab.gameObject.SetActive(_powerUpsStoreTab.gameObject.activeSelf);
        }
#endif
    }
}