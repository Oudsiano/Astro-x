using UnityEngine;

namespace LevelObjects
{
    [CreateAssetMenu(fileName = "Level Object Data")]
    public class LevelObjectData : ScriptableObject
    {
        [Header("Object type")]
        [SerializeField] public LevelObjectType Type;
        [SerializeField] public MoveType MoveType;
        [SerializeField] public LevelDestroyableObject[] SpawnObjects;
        
        [Header("Object destroy")]
        [SerializeField] public float Health;
        [SerializeField] public int ScoresForDestroy;
        [SerializeField] [Range(0f, 1f)] public float ShieldPercentBonusForDestroy;
        
        [Header("Collision")]
        [SerializeField] [Range(0f, 1f)] public float CollisionToPlayerDamagePercent;
        [SerializeField] public int ScoresPenaltyOnCollision;

        [Header("Shooting")]
        [SerializeField] public float ShootCooldown;
        [SerializeField] [Range(0f, 1f)] public float ShootDamagePercent;
    }
}