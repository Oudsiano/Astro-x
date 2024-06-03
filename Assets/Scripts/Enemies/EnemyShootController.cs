using DI;
using LevelObjects;
using Services.DI;
using Services.ObjectPools;
using Services.SoundsSystem;
using UnityEngine;

namespace Enemies
{
    public class EnemyShootController : MonoBehaviour
    {
        [SerializeField] private Transform _firePoint;
        [SerializeField] private EnemyShell _shellPrefab;
        [SerializeField] private float _shootSpeedMultiplier;

        [Inject] private SoundsSystem _soundsSystem;
        
        private LevelObjectData _enemyData;
    
        private bool _canShoot;
        private float _reloadTime;
        private float _reloadTimer;

        public void Initialize(LevelObjectData data, float shootCooldown)
        {
            GameContainer.InjectToInstance(this);

            if (_shootSpeedMultiplier != 0)
                shootCooldown /= _shootSpeedMultiplier;
            
            _enemyData = data;
            _reloadTime = shootCooldown;
        }

        private void Start()
        {
            _reloadTimer = _reloadTime;
        }

        private void Update()
        {
            if (!_canShoot)
            {
                UpdateReloadTimer();
            }
            else
            {
                var shell = PrefabMonoPool<EnemyShell>.GetPrefabInstance(_shellPrefab);
                if (shell == null) return;
                
                shell.transform.SetPositionAndRotation(_firePoint.position, _firePoint.rotation);
                shell.DamagePercent = _enemyData.ShootDamagePercent;
                
                _canShoot = false;
                _reloadTimer += _reloadTime;
            }
        }

        private void UpdateReloadTimer()
        {
            _reloadTimer -= Time.deltaTime;
            if (_reloadTimer <= 0) _canShoot = true;
        }
    }
}
