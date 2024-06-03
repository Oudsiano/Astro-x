using UnityEngine;

namespace LevelObjects
{
    public abstract class ObjectsSpawnerBase : MonoBehaviour
    {
        public abstract void Initialize();
        public abstract void StartSpawn(int count, LevelObjectData data, float shootCooldown, float health, float speedMultiplier);
        public abstract void Dispose();
        public abstract void CheckObjects(int remainingCount);
    }
}