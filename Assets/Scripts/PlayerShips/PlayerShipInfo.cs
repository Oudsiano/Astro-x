using System;
using UnityEngine;

namespace PlayerShips
{
    [Serializable]
    public class PlayerShipInfo
    {
        [NonSerialized] public int Id;
        [SerializeField] public bool OpenOnAllLevelsCompleted;
        [SerializeField] public string Name;
        [SerializeField] public Sprite Sprite;
        [SerializeField] public PlayerShipContainer ShipContainer;
        [SerializeField] public int Cost;
        [SerializeField] public bool UseGrayScale;
        [SerializeField] public Color ProjectileColor;
        [SerializeField] public EGA_Laser LaserPrefab;

        [SerializeField] public bool AlwaysBoostedLaser;
        [SerializeField] public float Damage;
        [SerializeField] public float ReloadTime;
        [SerializeField] public float ProjectileSpeed;

        [SerializeField] public float MaxHp;
    }
}