using Player.PowerUps;
using Services.DI;
using UnityEngine;

namespace Startup.InGame
{
    public class PowerUpsInitializer : InitializerBase
    {
        [SerializeField] private PowerUpManager _powerUpManager;
        
        public override void Initialize()
        {
            var powerUpManager = GameContainer.InstantiateAndResolve(_powerUpManager);
            GameContainer.InGame.Register(powerUpManager);
        }

        public override void Reinitialize()
        {
            if (GameContainer.InGame == null) return;
            if (!GameContainer.InGame.CanResolve<PowerUpManager>()) return;

            var powerUpManager = GameContainer.InGame.Resolve<PowerUpManager>();
            powerUpManager.ResetTimers();
        }
    }
}