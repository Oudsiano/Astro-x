using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Player.PowerUps;
using UnityEngine;

namespace PlayerProgress
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerProgressManager
    {
        private const string PlayerProgressPrefsKey = "PlayerProgress";

        public PlayerProgressData Data;
        
        public void LoadProgress()
        {
            if (!PlayerPrefs.HasKey(PlayerProgressPrefsKey))
            {
                CreateDefault();
                return;
            }

            string json = PlayerPrefs.GetString(PlayerProgressPrefsKey);
            try
            {
                Data = JsonConvert.DeserializeObject<PlayerProgressData>(json);
                Data.Initialize();
            }
            catch (Exception)
            {
                CreateDefault();
            }
        }

        public void SaveProgress()
        {
            Data.Save();
            string json = JsonConvert.SerializeObject(Data);
            PlayerPrefs.SetString(PlayerProgressPrefsKey, json);
        }

        private void CreateDefault()
        {
            Data = new PlayerProgressData
            {
                Money = 0,
                SelectedShip = 0,
                BoughtShips = new List<int> { 0 },
                CompletedLevels = new List<int>(),
                PowerUps = new List<BoughtPowerUpContainer>(),
                BoughtPowerUps = new Dictionary<PowerUpType, int>(),
            };
            Data.Initialize();
        }
    }
}