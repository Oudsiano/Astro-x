using DI;
using PlayerProgress;
using Services;
using Services.WindowsSystem;
using Startup;
using TMPro;
using Ui.Windows.Extra;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows
{
    public class MainMenuWindow : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private TextMeshProUGUI _shipNameText;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _hangarButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _creditsButton;
        [SerializeField] private ShipPreviewPanel _preview;
        
        [Inject] private PlayerProgressManager _progressManager;
        [Inject] private GameInitializer _gameInitializer;
        [Inject] private GameInfoContainer _gameInfoContainer;
        
        private void Start()
        {
            _moneyText.text = _progressManager.Data.Money.ToString();
            _preview.SpawnShip(_gameInfoContainer.CurrentShip);
            _shipNameText.text = _gameInfoContainer.CurrentShip.Name;
            
            _playButton.onClick.AddListener(Play);
            _settingsButton.onClick.AddListener(() =>
            {
                windowsSystem.CreateWindow<SettingsWindow>();
            });
            _shopButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                windowsSystem.CreateWindow<ShipsStoreWindow>();
            });
            _hangarButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                windowsSystem.CreateWindow<ShipsHangarWindow>();
            });
            _quitButton.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
            
            _creditsButton.onClick.AddListener(() =>
            {
                windowsSystem.CreateWindow<CreditsWindow>();
            });
        }

        private void OnEnable()
        {
            if (_progressManager != null && _progressManager.Data != null)
                _moneyText.text = _progressManager.Data.Money.ToString();
            
            if (_gameInfoContainer != null && _gameInfoContainer.CurrentShip != null)
            {
                _preview.SpawnShip(_gameInfoContainer.CurrentShip);
                _shipNameText.text = _gameInfoContainer.CurrentShip.Name;
            }
            
            _preview.gameObject.SetActive(true);
            _preview.Enter();
        }

        private void Play()
        {
            _preview.Exit(() =>
            {
                _preview.gameObject.SetActive(false);
                gameObject.SetActive(false);
                windowsSystem.CreateWindow<ChooseLevelWindow>();
            });
        }
    }
}