using Environment;
using Levels;
using UnityEngine;
using Utils;

namespace LevelObjects
{
    public class Planet : MonoBehaviour
    {
        private static readonly int MainTex = Shader.PropertyToID("_BaseMap");

        [SerializeField] private bool _useSelfTexture = true;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Rotator _rotator;

        private bool _stopOnEnd;
        private float _speed;
        private float _timer;
        private LevelInfo _levelInfo;
        
        public void Initialize(LevelInfo levelInfo, float speed)
        {
            _speed = speed;
            _levelInfo = levelInfo;
            
            _rotator.OverrideSpeed(levelInfo.Planet.RotationSpeed);
            
            if (!_useSelfTexture)
                _renderer.material.SetTexture(MainTex, levelInfo.Planet.Texture);
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            float t = _timer / _levelInfo.PlanetLeaveTime;
            t = Mathf.Clamp01(t);

            float sizeMultiplier = _levelInfo.PlanetSizeCurve.Evaluate(t);
            if (_levelInfo.Planet.Size > 0 && sizeMultiplier > 0)
                transform.localScale = Vector3.one * (_levelInfo.Planet.Size * sizeMultiplier);
            
            if (t >= 1 && _levelInfo.StopPlanetOnEnd)
                return;
            
            transform.AddYPosition(-_speed * Time.deltaTime);
            if (transform.position.y < -100)
                Destroy(gameObject);
        }
    }
}