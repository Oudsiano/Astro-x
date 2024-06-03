using Services.DI;
using Services.WindowsSystem;
using Ui.Windows;

namespace Startup.InGame
{
    public class GameUiInitializer : InitializerBase
    {
        public override void Initialize()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateWindow<InGameUI>();
        }

        public override void Dispose()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyWindow<InGameUI>();
        }

        public override void Reinitialize()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyWindow<InGameUI>();
            windowsSystem.CreateWindow<InGameUI>();
        }
    }
}