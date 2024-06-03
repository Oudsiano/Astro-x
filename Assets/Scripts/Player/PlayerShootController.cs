using Damage;
using DI;
using Player.NewPlayer;
using PlayerShips;
using Services.DI;
using Services.ObjectPools;
using Services.SoundsSystem;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerShootController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerProjectile _playerProjectile;
        [SerializeField] private MovingBorders _borders;
        [SerializeField] private float _minLaserWidth;
        [SerializeField] private float _maxLaserWidth;
        [SerializeField] private AudioSource _boostedLaserSource;
        [SerializeField] private LayerMask _shootMask;

        [Inject] private SoundsSystem _soundsSystem;
        
        private bool _powerMode;
        private float _shootTimer;

        private bool _damageBoosted;
        private float _boostDamageMultiplier;

        private bool _alwaysBoostedLaser;
        private float _damage;
        private float _projectileSpeed;
        private float _reloadTime;

        private Transform _firePoint;
        
        private Damageable _targetObject;

        private bool _useGrayScale;
        private Color _projectileColor;
        
        private GameObject _laserGO;
        private EGA_Laser _laser;
        private GameObject _impactEffect;
        private GameObject _shootEffect;

        public void Initialize(PlayerShipContainer shipContainer, PlayerShipInfo shipInfo)
        {
            GameContainer.InjectToInstance(this);

            _laser = Instantiate(shipInfo.LaserPrefab, _firePoint);
            _laserGO = _laser.gameObject;

            _impactEffect = _laser.HitEffect;
            _shootEffect = _laser.ShootEffect;
            
            _firePoint = shipContainer.FirePoint;

            _damage = shipInfo.Damage;
            _reloadTime = shipInfo.ReloadTime;
            _projectileSpeed = shipInfo.ProjectileSpeed;
            
            _useGrayScale = shipInfo.UseGrayScale;
            _projectileColor = shipInfo.ProjectileColor;
            
            // _laserLinePositions = new Vector3[2];

            _alwaysBoostedLaser = shipInfo.AlwaysBoostedLaser;
            
            if (_alwaysBoostedLaser)
                BoostDamage(1f, true);
            else
                DisableBoost();
        }

        private void OnDestroy()
        {
            if (_laserGO != null)
                Destroy(_laserGO);
        }

        public void BoostDamage(float damage, bool byDefault = false)
        {
            _damageBoosted = true;
            _boostDamageMultiplier = damage;
            
            if (_laserGO != null)
                _laserGO.SetActive(true);
            
            if (_boostedLaserSource != null && !byDefault)
                _boostedLaserSource.Play();
        }

        public void DisableBoost()
        {
            _boostDamageMultiplier = 1f;
            
            if (_boostedLaserSource != null)
                _boostedLaserSource.Stop();
            
            if (_alwaysBoostedLaser)
                return;
            
            _damageBoosted = false;
            if (_laserGO != null)
                _laserGO.SetActive(false);
            
            if (_impactEffect != null)
                _impactEffect.SetActive(false);
        }

        private void Update()
        {
            if (_damageBoosted)
                UpdateLaser();
            
            _shootTimer += Time.deltaTime;
            if (_shootTimer < _reloadTime)
                return;
            
            _shootTimer -= _reloadTime;
            Shoot();
        }

        private void UpdateLaser()
        {
            var shootPoint = _firePoint.position;
            
            bool hitTriggers = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = false;
            var hit = Physics2D.Raycast(shootPoint, Vector2.up, 100f, _shootMask);
            Physics2D.queriesHitTriggers = hitTriggers;

            var targetPoint = shootPoint + Vector3.up * 30f;

            _targetObject = null;
            if (hit.collider != null)
            {
                targetPoint = hit.point;
                if (hit.rigidbody != null)
                {
                    var damageable = hit.collider.GetComponentInParent<Damageable>();
                    if (damageable != null)
                        _targetObject = damageable;
                }
            }
            
            _laser.SetPoints(shootPoint, targetPoint);
            _shootEffect.transform.position = shootPoint;
            _impactEffect.SetActive(_targetObject != null);
        }

        private void Shoot()
        {
            if (_damageBoosted)
            {
                if (_targetObject != null)
                    _targetObject.Damage(_damage * _boostDamageMultiplier, DamageType.Laser);
                
                return;
            }
            
            if (_firePoint == null) return;
            
            var projectile = PrefabMonoPool<PlayerProjectile>.GetPrefabInstance(_playerProjectile);
            if (projectile == null) return;
            
            projectile.SetColor(_projectileColor, _useGrayScale);
            projectile.transform.position = _firePoint.position.AddY(0.5f);
            projectile.ProjectileSpeed = _projectileSpeed;
            projectile.Borders = _borders;
            projectile.IsActive = true;
            projectile.Damage = _damage;
        }
    }
}