using UnityEngine;

namespace Imports.BreakableAsteroids.Scripts
{
    public class FractureShatter : MonoBehaviour
    {
        private const float Force = 300f;
        public const float DestroyTime = 1f;

        private float _timer;
        
        private void Start()
        {
            var bodies = GetComponentsInChildren<Rigidbody>();
            foreach (var body in bodies)
            {
                var direction = body.transform.position - transform.position;
                direction.z = 0f;
                if (direction == Vector3.zero)
                    direction = Vector3.right;
                
                body.AddForce(direction * Force);
            }
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > DestroyTime)
                Destroy(gameObject);
        }
    }
}