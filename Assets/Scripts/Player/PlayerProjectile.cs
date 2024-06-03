using Damage;
using Enemies;
using Imports.SpaceShooterTemplate.Scripts;
using Player.NewPlayer;
using Services.ObjectPools;
using UnityEngine;

namespace Player
{
    public class PlayerProjectile : MonoBehaviour
    {
        public bool IsActive;
        
        [HideInInspector] public float Damage;
        [HideInInspector] public float ProjectileSpeed;
        [HideInInspector] public MovingBorders Borders;
        
        [SerializeField] private Sprite _usualSprite;
        [SerializeField] private Sprite _grayScaleSprite;
        [SerializeField] private VisualEffect _impactEffect;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private bool _returnNextFrame;

        public void SetColor(Color color, bool useGrayScale)
        {
            _spriteRenderer.sprite = useGrayScale ? _usualSprite : _grayScaleSprite;
            _spriteRenderer.color = color;
        }
        
        private void Update()
        {
            // ReSharper disable once Unity.InefficientPropertyAccess
            transform.position += transform.up * (ProjectileSpeed * Time.deltaTime);
            
            if (_returnNextFrame)
            {
                PrefabMonoPool<PlayerProjectile>.ReturnInstance(this);
                return;
            }

            if (transform.position.x > Borders.MaxX || transform.position.x < Borders.MinX ||
                transform.position.y > Borders.MaxY || transform.position.y < Borders.MinY)
                _returnNextFrame = true;
        }

        private void OnDisable()
        {
            _returnNextFrame = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsActive) return;
            
            var enemyShell = other.GetComponentInParent<EnemyShell>();
            if (enemyShell != null)
            {
                enemyShell.Destroy();
                IsActive = false;
                PrefabMonoPool<PlayerProjectile>.ReturnInstance(this);
                return;
            }
            
            if (other.attachedRigidbody == null) return;
            
            var player = other.attachedRigidbody.GetComponent<PlayerController>();
            if (player != null) return;
            
            var damageable = other.gameObject.GetComponentInParent<Damageable>();
            if (damageable != null)
                damageable.Damage(Damage, DamageType.Laser);

            var impactPrefab = PrefabMonoPool<VisualEffect>.GetPrefabInstance(_impactEffect);
            impactPrefab.transform.SetPositionAndRotation(other.ClosestPoint(transform.position), transform.rotation);
            
            IsActive = false;
            PrefabMonoPool<PlayerProjectile>.ReturnInstance(this);
        }
    }
}