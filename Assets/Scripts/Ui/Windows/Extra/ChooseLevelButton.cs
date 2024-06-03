using System;
using Levels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows.Extra
{
    public enum LevelButtonState
    {
        Completed,
        Available,
        Unavailable,
    }
    
    public class ChooseLevelButton : MonoBehaviour
    {
        public event Action<LevelInfo> OnPressed;
        public LevelButtonState State;
        
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _completedBg;
        [SerializeField] private GameObject _notCompletedBg;
        [SerializeField] private GameObject _unavailableBg;

        public void Initialize(LevelInfo levelInfo)
        {
            _levelText.text = levelInfo.LevelName;
            _button.onClick.AddListener(() =>
            {
                if (State != LevelButtonState.Unavailable)
                    OnPressed?.Invoke(levelInfo);
            });
        }

        public void SetState(LevelButtonState state)
        {
            State = state;
            _completedBg.SetActive(state == LevelButtonState.Completed);
            _notCompletedBg.SetActive(state == LevelButtonState.Available);
            _unavailableBg.SetActive(state == LevelButtonState.Unavailable);
        }
    }
}