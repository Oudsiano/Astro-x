using System;
using DI;
using Levels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Ui
{
    public class LoadingScreen : MonoBehaviour
    {
        public Action OnPlayPressed;

        public bool Active { set => gameObject.SetActive(value); }
        
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private GameObject _loadingIndicator;
        [SerializeField] private GameObject _loadedIndicator;
        [SerializeField] private Button _playButton;

        [Inject] private SpaceFactsInfo _spaceFactsInfo;

        private void Awake()
        {
            _playButton.onClick.AddListener(() => OnPlayPressed?.Invoke());
        }

        public void UpdateFacts(bool factsEnabled)
        {
            _description.gameObject.SetActive(factsEnabled);
            _description.text = _spaceFactsInfo.Facts.GetRandom();
        }

        public void SetLoadedState(bool state)
        {
            _loadingIndicator.SetActive(!state);
            _loadedIndicator.SetActive(state);
            _playButton.gameObject.SetActive(state);
        }
    }
}