using System.Collections.Generic;
using Damage;
using DI;
using LevelObjects.Messages;
using Services;
using Services.DI;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace LevelObjects.Spawners
{
    public class LevelObjectsSpawner : ObjectsSpawnerBase
    {
        [Header("Objects spawn")]
        [SerializeField] private int _maxActiveObjects;
        [SerializeField] private Vector2 _minMaxTimeToSpawn;
        [SerializeField] private float _maxXOffset;
        [SerializeField] private float _yToDespawn;
        
        [Header("Debug")]
        [SerializeField] private float _timer;
        [SerializeField] private bool _canSpawn;
        [SerializeField] private int _remainingCount;

        [Inject] private MessageBroker _messageBroker;

        private float _health;
        private float _shootCooldown;
        private float _speedMultiplier;
        private LevelObjectData _data;
        private List<LevelDestroyableObject> _spawnedObjects;
        private Queue<LevelDestroyableObject> _preparedObjects;
        private List<MoveType> _moveTypes;

        public override void Initialize()
        {
            GameContainer.InjectToInstance(this);
            
            _spawnedObjects = new List<LevelDestroyableObject>();
            _preparedObjects = new Queue<LevelDestroyableObject>();
            _moveTypes = new List<MoveType>();
            SetTimeToSpawn();
        }

        public override void StartSpawn(int count, LevelObjectData data, float shootCooldown, float health, float speedMultiplier)
        {
            Debug.Log($"Level spawner Instance {gameObject.GetInstanceID()} - Starting spawning {count} objects of type {data.Type}");
            _remainingCount = count;
            _canSpawn = true;
            _data = data;
            _health = health;
            _shootCooldown = shootCooldown;
            _speedMultiplier = speedMultiplier;
            
            _moveTypes.Clear();
            if ((data.MoveType & MoveType.Down) == MoveType.Down)
                _moveTypes.Add(MoveType.Down);
            if ((data.MoveType & MoveType.DiagonalLeft) == MoveType.DiagonalLeft)
                _moveTypes.Add(MoveType.DiagonalLeft);
            if ((data.MoveType & MoveType.DiagonalRight) == MoveType.DiagonalRight)
                _moveTypes.Add(MoveType.DiagonalRight);
            
            if (_moveTypes.Count == 0)
                _moveTypes.Add(MoveType.Down);
            
            SetTimeToSpawn();

            for (int i = 0; i < count; i++)
            {
                var objectToSpawn = data.SpawnObjects.GetRandom();
                var spawnedObject = GameContainer.InstantiateAndResolve(objectToSpawn);
            
                spawnedObject.InitializeWithData(_data, health, shootCooldown);
                spawnedObject.gameObject.SetActive(false);
                _preparedObjects.Enqueue(spawnedObject);
            }
        }

        public override void Dispose()
        {
            Debug.Log($"Level spawner Instance {gameObject.GetInstanceID()} - stop spawning objects");
            _remainingCount = 0;
            _canSpawn = false;
            
            while (_preparedObjects.TryDequeue(out var spawnedObject))
            {
                if (spawnedObject != null)
                    Destroy(spawnedObject.gameObject);
            }

            foreach (var spawnedObject in _spawnedObjects)
            {
                if (spawnedObject != null)
                    Destroy(spawnedObject.gameObject);
            }
            
            _preparedObjects.Clear();
            _spawnedObjects.Clear();
        }

        public override void CheckObjects(int remainingCount)
        {
            _remainingCount = remainingCount;
            if (remainingCount > 0)
                _canSpawn = true;
            
            SpawnRandomObject();
        }

        private void Update()
        {
            if (!_canSpawn) return;
            
            UpdateSpawnedObjects();
            
            if (_spawnedObjects.Count >= _maxActiveObjects || !_canSpawn || _remainingCount <= 0) return;
            
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            
            SpawnRandomObject();
            SetTimeToSpawn();
        }

        private void UpdateSpawnedObjects()
        {
            if (_spawnedObjects == null) return;
            if (_preparedObjects == null) return;
            
            for (int i = _spawnedObjects.Count - 1; i >= 0; i--)
            {
                var spawnedObject = _spawnedObjects[i];
                if (spawnedObject == null)
                {
                    Debug.Log($"Removing null prepared object {_data.Type}");
                    _spawnedObjects.RemoveAt(i);
                    return;
                }
                if (spawnedObject.transform == null)
                {
                    Debug.Log($"Removing null transform prepared object {_data.Type}");
                    _spawnedObjects.RemoveAt(i);
                    return;
                }
                
                if (spawnedObject.transform.position.y > _yToDespawn) continue;
                
                spawnedObject.gameObject.SetActive(false);
                spawnedObject.OnObjectDestroyed -= OnObjectDestroyed;
                
                _preparedObjects.Enqueue(spawnedObject);
                _spawnedObjects.RemoveAt(i);
            }
        }

        private void SetTimeToSpawn()
        {
            _timer = Random.Range(_minMaxTimeToSpawn.x, _minMaxTimeToSpawn.y);
        }

        private void SpawnRandomObject()
        {
            if (_remainingCount == 0)
            {
                Debug.Log($"Level spawner {_data.Type} Instance {gameObject.GetInstanceID()} - spawn request when remaining is 0, send destroy message");
                
                var message = new LevelObjectDestroyedMessage
                {
                    DamageType = DamageType.Collision,
                    Data = _data,
                };
                _messageBroker.Trigger(ref message);
            }
            
            LevelDestroyableObject spawnedObject;
            if (_preparedObjects.Count == 0)
            {
                if (_spawnedObjects.Count >= _remainingCount)
                    return;
                
                var objectToSpawn = _data.SpawnObjects.GetRandom();
                spawnedObject = GameContainer.InstantiateAndResolve(objectToSpawn);
                spawnedObject.InitializeWithData(_data, _health, _shootCooldown);
                Debug.Log($"Level spawner {_data.Type} Instance {gameObject.GetInstanceID()} - No objects prepared, spawn new object!");
            }
            else
            {
                spawnedObject = _preparedObjects.Dequeue();
                Debug.Log($"Spawn new prepared object {_data.Type}");
            }

            var moveType = _moveTypes.GetRandom();
            float x = moveType switch
            {
                MoveType.Down => Random.Range(-_maxXOffset, _maxXOffset),
                MoveType.DiagonalLeft => Random.Range(_maxXOffset * 2f, _maxXOffset * 3f),
                MoveType.DiagonalRight => Random.Range(-_maxXOffset * 3f, -_maxXOffset * 2f),
                _ => Random.Range(-_maxXOffset, _maxXOffset)
            };
            
            spawnedObject.transform.position = transform.position.WithX(x);
            
            var moveDown = spawnedObject.GetComponent<ObjectMoveDown>();
            moveDown.Initialize(moveType, _speedMultiplier);
            
            spawnedObject.gameObject.SetActive(true);
            spawnedObject.OnObjectDestroyed += OnObjectDestroyed;
            
            _spawnedObjects.Add(spawnedObject);
        }

        private void OnObjectDestroyed(LevelDestroyableObject target)
        {
            target.OnObjectDestroyed -= OnObjectDestroyed;
            _spawnedObjects.Remove(target);
            _remainingCount--;
        }
    }
}