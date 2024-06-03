using UnityEngine;

namespace Player.PowerUps
{
    [CreateAssetMenu(fileName = "PowerUpData")]
    public class PowerUpData : ScriptableObject
    {
        [SerializeField] public string Name;
        [SerializeField] public string Description;
        [SerializeField] public int Cost;
        [SerializeField] public PowerUpType Type;
        [SerializeField] public float ActiveTime;
        [SerializeField] public Sprite ShopSprite;
        [SerializeField] public Sprite GameSprite;
    }
}