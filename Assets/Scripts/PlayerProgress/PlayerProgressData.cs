using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Player.PowerUps;

namespace PlayerProgress
{
    [JsonObject]
    public class PlayerProgressData
    {
        [JsonProperty("bool")] public bool PlayedFirstLaunchMusic;
        [JsonProperty("money")] public int Money;
        [JsonProperty("selected_ship")] public int SelectedShip;
        [JsonProperty("bought_ships")] public List<int> BoughtShips;
        [JsonProperty("completed_levels")] public List<int> CompletedLevels;
        [JsonProperty("power_ups")] public List<BoughtPowerUpContainer> PowerUps;
        [JsonProperty("last-level")] public int LastCompletedLevel;
        [JsonProperty("completed")] public bool IsGameCompleted;
        
        [JsonIgnore] public Dictionary<PowerUpType, int> BoughtPowerUps;

        public void Initialize()
        {
            BoughtPowerUps = new Dictionary<PowerUpType, int>();
            foreach (var powerUp in PowerUps)
            {
                BoughtPowerUps[powerUp.Type] = powerUp.Count;
            }
        }

        public void AddMoney(int value)
        {
            SetMoney(Money + value);
        }

        public void SetMoney(int value)
        {
            Money = value;
        }

        public void Save()
        {
            BoughtShips = BoughtShips.Distinct().ToList();
            PowerUps.Clear();
            foreach (var powerUp in BoughtPowerUps)
            {
                PowerUps.Add(new BoughtPowerUpContainer
                {
                    PowerUpTypeId = (int) powerUp.Key,
                    Count = powerUp.Value,
                });
            }
        }

        public int GetPowerUpCount(PowerUpType type) => BoughtPowerUps.GetValueOrDefault(type, 0);
    }

    [JsonObject]
    public class BoughtPowerUpContainer
    {
        [JsonIgnore]
        public PowerUpType Type
        {
            get => (PowerUpType)PowerUpTypeId;
            set => PowerUpTypeId = (int)value;
        }
        
        [JsonProperty("type_id")] public int PowerUpTypeId;
        [JsonProperty("count")] public int Count;
    }
}