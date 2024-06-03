using UnityEngine;

namespace Ui.Extra
{
    public class Pulse : MonoBehaviour
    {
        [SerializeField] private float _period;
        [SerializeField] private AnimationCurve _curve;

        private float _timer;
        private Vector3 _startScale;

        private void Start()
        {
            _startScale = transform.localScale;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > _period) _timer -= _period;

            float t = _timer / _period;
            float scaleMultiplier = _curve.Evaluate(t);
            transform.localScale = _startScale * scaleMultiplier;
        }
    }
}