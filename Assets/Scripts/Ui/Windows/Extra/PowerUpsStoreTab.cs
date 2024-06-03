using System.Collections.Generic;
using DI;
using Player.PowerUps;
using PlayerProgress;
using Services.DI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows.Extra
{
    public class PowerUpsStoreTab : MonoBehaviour
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _prevButton;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private GameObject _canBuyIndicator;
        [SerializeField] private GameObject _cannotBuyIndicator;
        [SerializeField] private Image _powerUpImage;
        [SerializeField] private ShipsStoreWindow _window;
        
        [Inject] private ShopPowerUpsTabData _powerUpsTabData;
        [Inject] private PlayerProgressManager _progressManager;

        private int _currentViewingPowerUp;

        private void Awake()
        {
            GameContainer.InjectToInstance(this);
        }

        private void Start()
        {
            _nextButton.onClick.AddListener(() =>
            {
                _currentViewingPowerUp++;
                _currentViewingPowerUp %= _powerUpsTabData.PowerUpsInShop.Length;
                ShowPowerUp();
            });
            
            _prevButton.onClick.AddListener(() =>
            {
                _currentViewingPowerUp--;
                if (_currentViewingPowerUp < 0) _currentViewingPowerUp += _powerUpsTabData.PowerUpsInShop.Length;
                ShowPowerUp();
            });
            
            _buyButton.onClick.AddListener(Buy);
        }

        private void OnEnable()
        {
            ShowPowerUp();
        }

        private void ShowPowerUp()
        {
            var powerUp = _powerUpsTabData.PowerUpsInShop[_currentViewingPowerUp];
            
            _powerUpImage.sprite = powerUp.ShopSprite;
            _nameText.text = powerUp.Name;
            _descriptionText.text = powerUp.Description;

            _countText.text = _progressManager.Data.BoughtPowerUps.GetValueOrDefault(powerUp.Type, 0).ToString();
            _costText.text = powerUp.Cost.ToString();

            bool canBuy = _progressManager.Data.Money >= powerUp.Cost;
            _canBuyIndicator.SetActive(canBuy);
            _cannotBuyIndicator.SetActive(!canBuy);
        }

        private void Buy()
        {
            var powerUp = _powerUpsTabData.PowerUpsInShop[_currentViewingPowerUp];
            if (powerUp.Cost > _progressManager.Data.Money) return;

            int count = _progressManager.Data.BoughtPowerUps.GetValueOrDefault(powerUp.Type, 0);
            _progressManager.Data.BoughtPowerUps[powerUp.Type] = count + 1;
            _progressManager.Data.AddMoney(-powerUp.Cost);
            _progressManager.SaveProgress();
            _window.UpdateMoney();
            
            ShowPowerUp();
        }
    }
}