using UnityEngine;

namespace Player.PowerUps
{
    [CreateAssetMenu(fileName = "ShopPowerUpsTabData")]
    public class ShopPowerUpsTabData : ScriptableObject
    {
        [SerializeField] public PowerUpData[] PowerUpsInShop;
    }
}