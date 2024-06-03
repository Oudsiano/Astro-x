using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows.Extra
{
    public class PowerUpButton : MonoBehaviour
    {
        public event Action OnClicked;
        
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private TextMeshProUGUI _countText;

        private int _seconds;

        private void Start()
        {
            _button.onClick.AddListener(() => OnClicked?.Invoke());
        }

        public void UpdateTimer(int seconds)
        {
            if (seconds == 0)
            {
                _timerText.gameObject.SetActive(false);
                _seconds = 0;
                return;
            }
            
            if (seconds == _seconds)
                return;

            _timerText.gameObject.SetActive(true);
            _timerText.text = seconds.ToString();
            _seconds = seconds;
        }

        public void SetCount(int value) => _countText.text = value.ToString();
    }
}