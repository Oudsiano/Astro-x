using UnityEngine;

namespace PlayerShips
{
    [CreateAssetMenu(fileName = "Player Ships Data")]
    public class PlayerShipsData : ScriptableObject
    {
        [SerializeField] public PlayerShipInfo[] PlayerShips;

        public void Initialize()
        {
            for (int i = 0; i < PlayerShips.Length; i++)
            {
                PlayerShips[i].Id = i;
            }
        }
    }
}