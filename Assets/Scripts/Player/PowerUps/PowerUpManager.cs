using System.Linq;
using DI;
using Player.PowerUps.Behaviours;
using PlayerProgress;
using UnityEngine;

namespace Player.PowerUps
{
    public class PowerUpManager : MonoBehaviour
    {
        [SerializeField] private PowerUpLaserBehaviour _laserBehaviour;
        [SerializeField] private PowerUpShieldBehaviour _shieldBehaviour;
        
        [Inject] private PlayerProgressManager _progressManager;
        [Inject] private ShopPowerUpsTabData _powerUpsTabData;

        public void EnablePowerUp(PowerUpType type)
        {
            int count = _progressManager.Data.GetPowerUpCount(type);
            if (count <= 0) return;

            count--;
            _progressManager.Data.BoughtPowerUps[type] = count;
            EnablePowerUp(_powerUpsTabData.PowerUpsInShop.FirstOrDefault(x => x.Type == type));
        }

        public void EnablePowerUp(PowerUpData data)
        {
            if (data.Type == PowerUpType.Laser)
            {
                _laserBehaviour.Enable(data);
            }
            else if (data.Type == PowerUpType.Shield)
            {
                _shieldBehaviour.Enable(data);
            }
        }

        public void ResetTimers()
        {
            _laserBehaviour.ResetTimers();
            _shieldBehaviour.ResetTimers();
        }
    }
}