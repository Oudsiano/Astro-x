using System;
using Cysharp.Threading.Tasks;
using DI;
using Levels;
using PlayerProgress;
using Services;
using Services.SoundsSystem;
using Services.WindowsSystem;
using Startup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Ui.Windows
{
    public class PlanetInfoSlidesWindow : WindowBase
    {
        public Action PlayPressed;
        
        [SerializeField] private Image _planetPreview;
        [SerializeField] private RectTransform _planetPreviewRectTransform;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Button _nextSlideButton;
        [SerializeField] private Button _prevSlideButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameLevelsData _levelsData;

        [Inject] private GameInitializer _gameInitializer;
        [Inject] private GameInfoContainer _gameInfoContainer;
        [Inject] private PlayerProgressManager _progressManager;
        [Inject] private SoundsSystem _soundsSystem;
        
        private int _slideId;
        private LevelInfo _levelInfo;

        private void Start()
        {
            _soundsSystem.PlayMusic(MusicType.LevelPreview);
            _nextSlideButton.onClick.AddListener(() =>
            {
                if (_slideId >= _levelInfo.Planet.InfoSlides.Count - 1)
                    return;
                
                _slideId++;
                ShowSlide();
            });
            
            _prevSlideButton.onClick.AddListener(() =>
            {
                if (_slideId <= 0) return;

                _slideId--;
                ShowSlide();
            });
            
            _playButton.onClick.AddListener(OnPlayPressed);
        }

        private void OnPlayPressed()
        {
            if (_slideId < _levelInfo.Planet.InfoSlides.Count - 1)
                return;

            if (_levelInfo.OnlyShowInfo)
            {
                if (!_progressManager.Data.CompletedLevels.Contains(_gameInfoContainer.CurrentLevel.Id))
                {
                    _progressManager.Data.CompletedLevels.Add(_gameInfoContainer.CurrentLevel.Id);
                    _progressManager.SaveProgress();
                }
                
                var nextLevelId = _levelInfo.Id + 1;
                if (nextLevelId >= _levelsData.Levels.Length)
                {
                    windowsSystem.DestroyWindow(this);
                    windowsSystem.CreateWindow<GameCompletedWindow>();
                }
                else
                {
                    var nextLevel = _levelsData.Levels[nextLevelId];
                    _gameInfoContainer.CurrentLevel = nextLevel;
                    if (nextLevel.ShowPlanetInfo && nextLevel.Planet != null && nextLevel.Planet.InfoSlides is { Count: > 0 })
                    {
                        SetLevel(nextLevel);
                    }
                    else
                    {
                        windowsSystem.DestroyWindow(this);
                        _gameInitializer.GoToMenuAndStartGame().Forget();
                    }
                }
            }
            else
            {
                windowsSystem.DestroyWindow(this);
                _gameInitializer.GoToMenuAndStartGame().Forget();
            }
        }

        public void SetLevel(LevelInfo levelInfo)
        {
            _levelInfo = levelInfo;
            _slideId = 0;
            ShowSlide();
        }

        private void ShowSlide()
        {
            var slide = _levelInfo.Planet.InfoSlides[_slideId];
            SetSprite(slide.PreviewSprite);
            
            _prevSlideButton.gameObject.SetActive(_slideId > 0);
            _nextSlideButton.gameObject.SetActive(_slideId < _levelInfo.Planet.InfoSlides.Count - 1);
            _playButton.gameObject.SetActive(_slideId >= _levelInfo.Planet.InfoSlides.Count - 1);

            _description.text = slide.TextAsset.text;
            _scrollRect.normalizedPosition = Vector2.one;
        }

        private void SetSprite(Sprite sprite)
        {
            _planetPreview.sprite = sprite;
            float widthToHeightRatio = sprite.texture.width * 1f / sprite.texture.height;
            var rect = _planetPreviewRectTransform.rect;
            float newWidth = rect.height * widthToHeightRatio;
            _planetPreviewRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }
}