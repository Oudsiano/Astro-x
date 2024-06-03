using System;
using Damage;
using DI;
using Enemies;
using LevelObjects.Messages;
using Player;
using Services;
using Services.DI;
using Services.ObjectPools;
using Ui.Windows;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace LevelObjects
{
    public class LevelDestroyableObject : MonoBehaviour
    {
        public event Action<LevelDestroyableObject> OnObjectDestroyed;
        public LevelObjectData Data { get; private set; }
        
        [SerializeField] private Damageable _damageable;
        [SerializeField] private HealthStatus _healthStatus;
        [SerializeField] private EnemyShootController _shootController;
        [SerializeField] [Range(0f, 1f)] private float _dropChance;
        [SerializeField] private Collectable[] _dropCollectables;

        [Inject] private MessageBroker _messageBroker;
        
        private void Start()
        {
            GameContainer.InjectToInstance(this);
            _damageable.OnDestroyed += OnDestroyed;
        }

        public void InitializeWithData(LevelObjectData data, float health, float shootCooldown)
        {
            Data = data;
            _damageable.Initialize(_healthStatus, health);
            
            if (_shootController != null)
                _shootController.Initialize(data, shootCooldown);
        }

        private void OnDestroyed(DamageType damageType)
        {
            if (_damageable == null) return;
            
            _damageable.OnDestroyed -= OnDestroyed;
            OnObjectDestroyed?.Invoke(this);

            var message = new LevelObjectDestroyedMessage
            {
                DamageType = damageType,
                Data = Data,
            };
            
            _messageBroker ??= GameContainer.Common.Resolve<MessageBroker>();
            _messageBroker.Trigger(ref message);
            
            if (_dropChance < Random.value) return;
            if (_dropCollectables == null || _dropCollectables.Length == 0) return;
            
            var drop = _dropCollectables.GetRandom();
            if (drop == null) return;

            var dropInstance = PrefabMonoPool<Collectable>.GetPrefabInstance(drop);
            if (dropInstance == null) return;
            
            // ReSharper disable once Unity.InefficientPropertyAccess
            dropInstance.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var player = other.collider.GetComponentInParent<PlayerController>();
            if (player == null)
                return;
            
            _damageable.Destruct(DamageType.Collision);
        }
    }
}