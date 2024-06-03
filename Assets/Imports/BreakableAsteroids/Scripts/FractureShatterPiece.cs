using UnityEngine;

namespace Imports.BreakableAsteroids.Scripts
{
    public class FractureShatterPiece : MonoBehaviour
    {
        private static Vector3 _endScale = Vector3.one * 0.01f;

        private float _timer;
        private Vector3 _startScale;

        private void Start()
        {
            _startScale = transform.localScale;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            float t = _timer / FractureShatter.DestroyTime;
            transform.localScale = Vector3.Lerp(_startScale, _endScale, t);
        }
    }
}