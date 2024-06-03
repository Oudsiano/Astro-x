using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DI;
using Levels;
using PlayerProgress;
using Services;
using Services.WindowsSystem;
using Startup;
using TMPro;
using Ui.Windows.Extra;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows
{
    public class ChooseLevelWindow : WindowBase
    {
        [SerializeField] private GameLevelsData _levelsData;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _openAllButton;
        [SerializeField] private Button _completeAllButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private TextMeshProUGUI _availableLevelText;
        [SerializeField] private ScrollRect _scrollRect;
        
        [SerializeField] private ChooseLevelButton _buttonPrefab;
        [SerializeField] private Transform _buttonsParent;
        
        [Inject] private GameInitializer _gameInitializer;
        [Inject] private GameInfoContainer _gameInfoContainer;
        [Inject] private PlayerProgressManager _progressManager;

        private int _availableLevelId;
        private LevelInfo _availableLevel;
        private List<ChooseLevelButton> _buttons;
        
        private void Start()
        {
            _buttons = new List<ChooseLevelButton>();
            bool unavailableButtons = false;
            _availableLevel = null;

            for (int i = 0; i < _levelsData.Levels.Length; i++)
            {
                var level = _levelsData.Levels[i];
                bool isCompleted = _progressManager.Data.CompletedLevels.Contains(level.Id);
                var button = Instantiate(_buttonPrefab, _buttonsParent);

                var state = LevelButtonState.Completed;
                if (unavailableButtons)
                {
                    state = LevelButtonState.Unavailable;
                }
                else if (!isCompleted)
                {
                    _availableLevel = level;
                    _availableLevelId = i;
                    state = LevelButtonState.Available;
                    unavailableButtons = true;
                }

                button.Initialize(level);
                button.SetState(state);
                button.OnPressed += SelectLevel;
                _buttons.Add(button);
            }
            
            if (_progressManager.Data.IsGameCompleted)
            {
                int levelId = _progressManager.Data.LastCompletedLevel;
                levelId = Mathf.Clamp(levelId + 1, 0, _levelsData.Levels.Length - 1);
                _availableLevel = _levelsData.Levels[levelId];
            }

            if (_availableLevel == null)
                _availableLevel = _levelsData.Levels[^1];
            
            if (_availableLevelId < 0)
                _availableLevelId = _levelsData.Levels.Length;
            
            _availableLevelText.text = _availableLevel.LevelName;
            _continueButton.onClick.AddListener(() => SelectLevel(_availableLevel));

            StartCoroutine(UpdateScroll());

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

#if UNITY_EDITOR
            _openAllButton.onClick.AddListener(() =>
            {
                foreach (var button in _buttons)
                {
                    if (button.State == LevelButtonState.Unavailable)
                        button.SetState(LevelButtonState.Available);
                }
            });
            
            _completeAllButton.onClick.AddListener(() =>
            {
                foreach (var level in _levelsData.Levels)
                {
                    if (!_progressManager.Data.CompletedLevels.Contains(level.Id))
                        _progressManager.Data.CompletedLevels.Add(level.Id);
                }
                
                foreach (var button in _buttons)
                {
                    button.SetState(LevelButtonState.Completed);
                }

                windowsSystem.DestroyWindow(this);
                windowsSystem.CreateWindow<GameCompletedWindow>();
            });
#else
            _openAllButton.enabled = false;
            _completeAllButton.enabled = false;
#endif
        }

        private void SelectLevel(LevelInfo levelInfo)
        {
            _progressManager.Data.LastCompletedLevel = Mathf.Max(0, levelInfo.Id - 1);
            _progressManager.SaveProgress();
            
            windowsSystem.DestroyWindow(this);
            _gameInfoContainer.CurrentLevel = levelInfo;
            
            if (levelInfo.ShowPlanetInfo && levelInfo.Planet != null && levelInfo.Planet.InfoSlides is { Count: > 0 })
            {
                var slidesWindow = windowsSystem.CreateWindow<PlanetInfoSlidesWindow>();
                slidesWindow.SetLevel(levelInfo);
            }
            else
            {
                _gameInitializer.StartGame().Forget();
            }
        }

        private IEnumerator UpdateScroll()
        {
            yield return new WaitForEndOfFrame();
            yield return null;
            
            _scrollRect.verticalNormalizedPosition = 1f - _availableLevelId * 1f / _levelsData.Levels.Length;
        }
    }
}