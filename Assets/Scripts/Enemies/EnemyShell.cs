using Damage;
using Imports.SpaceShooterTemplate.Scripts;
using Player;
using Player.NewPlayer;
using Services.ObjectPools;
using UnityEngine;

namespace Enemies
{
    public class EnemyShell : MonoBehaviour
    {
        [HideInInspector] public float DamagePercent;
        
        [SerializeField] private float _moveSpeed;
        [SerializeField] private VisualEffect _impactEffect;
        [SerializeField] private MovingBorders _movingBorders;
        
        private void Update()
        {
            transform.Translate(-Vector3.up * (_moveSpeed * Time.deltaTime));
            if (transform.position.x > _movingBorders.MaxX || transform.position.x < _movingBorders.MinX || transform.position.y > _movingBorders.MaxY || transform.position.y < _movingBorders.MinY)
                Destroy();
        }

        public void Destroy()
        {
            PrefabMonoPool<EnemyShell>.ReturnInstance(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.attachedRigidbody == null) return;
            
            var player = other.attachedRigidbody.GetComponent<PlayerController>();
            if (player == null) return;
            
            var damageable = other.gameObject.GetComponentInParent<Damageable>();
            if (damageable != null)
                damageable.Damage(DamagePercent * damageable.InitialHp, DamageType.Collision);

            var impactPrefab = PrefabMonoPool<VisualEffect>.GetPrefabInstance(_impactEffect);
            impactPrefab.transform.SetPositionAndRotation(other.ClosestPoint(transform.position), transform.rotation);
            Destroy();
        }
    }
}
