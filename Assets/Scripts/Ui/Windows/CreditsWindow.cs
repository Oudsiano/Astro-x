using Services.WindowsSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows
{
    public class CreditsWindow : WindowBase
    {
        [SerializeField] private Button _backButton;

        private void Awake()
        {
            _backButton.onClick.AddListener(() =>
            {
                windowsSystem.DestroyWindow(this);
            });
        }
    }
}