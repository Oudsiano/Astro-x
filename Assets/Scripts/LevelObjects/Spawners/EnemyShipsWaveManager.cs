using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace LevelObjects.Spawners
{
    public class EnemyShipsWaveManager : ObjectsSpawnerBase
    {
        private Vector3 EnemyMoveZoneCenter => transform.position + _enemyMoveZoneOffset;
        private Vector3 EnemyMoveZoneSize => new(_enemyMoveZoneWidth, _enemyMoveZoneHeight, 0f);
    
        [Header("Timers")]
        [SerializeField] private float _startDelay;
        [SerializeField] private float _timeBetweenWaves;
        [SerializeField] private bool _waitForLastEnemy;
    
        [Header("Spawn zones")]
        [SerializeField, Range(1, 10)] private float _enemyMoveZoneWidth;
        [SerializeField, Range(1, 10)] private float _enemyMoveZoneHeight;
        [SerializeField] private Vector3 _enemyMoveZoneOffset;
        [SerializeField] private Transform[] _spawnPoints;
    
        [Header("Waves")]
        [SerializeField] private Wave _wave;

        private bool _canSpawn;
        private int _remainingCount;
    
        private float _countdownToNewWave;
        private int _enemiesAlive;

        private float _shootCooldown;
        private float _health;
        private float _speedMultiplier;
        private LevelObjectData _enemyData;
        private List<EnemyMoveController> _spawnedEnemies;

        public override void Initialize()
        {
            _spawnedEnemies = new List<EnemyMoveController>();
        }
    
        public override void StartSpawn(int count, LevelObjectData data, float shootCooldown, float health, float speedMultiplier)
        {
            foreach (var spawnObject in data.SpawnObjects)
            {
                var enemy = spawnObject.GetComponent<EnemyMoveController>();
                if (enemy == null)
                {
                    Debug.LogError($"Префаб врага {spawnObject.name} неправильно настроен! На нём нет скрипта EnemyMoveController!");
                    return;
                }
            }
            
            Debug.Log($"Enemy spawner Instance {gameObject.GetInstanceID()} - Starting spawning {count} enemies of type {data.Type}");
            _countdownToNewWave = _startDelay;
            _enemyData = data;
            _canSpawn = true;
            _remainingCount = count;
            _shootCooldown = shootCooldown;
            _health = health;
            _speedMultiplier = speedMultiplier;
            _enemiesAlive = 0;
        }

        public override void Dispose()
        {
            Debug.Log($"Enemy spawner Instance {gameObject.GetInstanceID()} - Stop spawning enemies");
            _canSpawn = false;
            _remainingCount = 0;
        
            StopAllCoroutines();
            if (_spawnedEnemies is not { Count: > 0 })
                return;
        
            foreach (var spawnedEnemy in _spawnedEnemies)
            {
                if (spawnedEnemy != null)
                    Destroy(spawnedEnemy.gameObject);
            }

            _spawnedEnemies.Clear();
        }

        public override void CheckObjects(int remainingCount)
        {
            
        }

        private void Update()
        {
            if (!_canSpawn || _remainingCount <= 0)
                return;
        
            if (_waitForLastEnemy && _enemiesAlive > 0)
                return;

            if (_countdownToNewWave <= 0)
            {
                StartCoroutine(SpawnWave(_wave));
                _countdownToNewWave = _timeBetweenWaves;
            }

            _countdownToNewWave -= Time.deltaTime;
            _countdownToNewWave = Mathf.Clamp(_countdownToNewWave, 0, _timeBetweenWaves);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void SpawnEnemy()
        {
            int i = Random.Range(0, _spawnPoints.Length);
            
            var enemyLevelObject = Instantiate(_enemyData.SpawnObjects.GetRandom(), _spawnPoints[i].position, Quaternion.identity);
            enemyLevelObject.InitializeWithData(_enemyData, _health, _shootCooldown);
            enemyLevelObject.OnObjectDestroyed += OnEnemyDestroyed;

            var newEnemy = enemyLevelObject.GetComponent<EnemyMoveController>();
            newEnemy.Init(EnemyMoveZoneCenter, EnemyMoveZoneSize, _speedMultiplier);
            _spawnedEnemies.Add(newEnemy);
            _enemiesAlive++;

            var spawnedColliders = newEnemy.Colliders;
            foreach (var spawnedEnemy in _spawnedEnemies)
            {
                if (spawnedEnemy == newEnemy) continue;
                var colliders = spawnedEnemy.Colliders;
                foreach (var spawnedCollider in spawnedColliders)
                {
                    foreach (var enemyCollider in colliders)
                    {
                        Physics.IgnoreCollision(spawnedCollider, enemyCollider);
                    }
                }
            }
        }

        private void OnEnemyDestroyed(LevelDestroyableObject enemyObject)
        {
            enemyObject.OnObjectDestroyed -= OnEnemyDestroyed;
            var moveController = enemyObject.GetComponent<EnemyMoveController>();
            _spawnedEnemies.Remove(moveController);
            _remainingCount--;
            _enemiesAlive--;
        }

        private IEnumerator SpawnWave(Wave wave)
        {
            int spawnedCount = 0;
        
            if (spawnedCount >= _remainingCount)
                yield break;

            for (int j = 0; j < wave.Count; j++)
            {
                SpawnEnemy();
                spawnedCount++;
                if (spawnedCount >= _remainingCount)
                    break;
                
                yield return new WaitForSeconds(wave.TimeBetweenEnemies);
            }
        }
    
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.8f, 0f, 0f, 0.4f);
            Gizmos.DrawCube(EnemyMoveZoneCenter, EnemyMoveZoneSize);
        }
#endif
    }
}