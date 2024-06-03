using Levels;
using PlayerShips;
using UnityEngine;

namespace Services
{
    public class GameInfoContainer
    {
        public const int MissionsBetweenAds = 2;
        
        private const float AdPowerUpRewardsIntervalSeconds = 60f * 2f;

        public bool CanGivePowerUp => Time.time - LastAdReceivedRewardTime > AdPowerUpRewardsIntervalSeconds;
        
        public int MissionsToShowAd;
        public float LastAdReceivedRewardTime;
        
        public LevelInfo CurrentLevel;
        public PlayerShipInfo CurrentShip;
    }
}