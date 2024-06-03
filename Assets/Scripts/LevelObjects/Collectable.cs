using DI;
using Player;
using Player.PowerUps;
using Services.DI;
using Services.ObjectPools;
using UnityEngine;

namespace LevelObjects
{
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private PowerUpData _data;
        [SerializeField] private ObjectMoveDown _moveDown;
        
        [Inject] private PowerUpManager _powerUpManager;

        private void Awake()
        {
            GameContainer.InjectToInstance(this);
            _moveDown.Initialize(MoveType.Down, 1f);
        }

        private void Update()
        {
            if (transform.position.y < -100)
                PrefabMonoPool<Collectable>.ReturnInstance(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.attachedRigidbody.GetComponent<PlayerController>();
            if (player == null) return;
            
            _powerUpManager.EnablePowerUp(_data);
            PrefabMonoPool<Collectable>.ReturnInstance(this);
        }
    }
}