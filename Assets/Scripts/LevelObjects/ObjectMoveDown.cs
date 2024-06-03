using System;
using UnityEngine;

namespace LevelObjects
{
    public class ObjectMoveDown : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private float _resultSpeed;
        private Vector3 _moveDirection;
        
        public void Initialize(MoveType moveType, float speedMultiplier)
        {
            if (speedMultiplier == 0f) speedMultiplier = 1f;
            _resultSpeed = _speed * speedMultiplier;
            _moveDirection = moveType switch
            {
                MoveType.Down => Vector3.down,
                MoveType.DiagonalLeft => (Vector3.down + Vector3.left).normalized,
                MoveType.DiagonalRight => (Vector3.down + Vector3.right).normalized,
                _ => Vector3.down
            };
        }
        
        private void Update()
        {
            transform.position += _moveDirection * (_resultSpeed * Time.deltaTime);
        }
    }
}