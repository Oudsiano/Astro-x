using LevelObjects;
using Services;
using Services.DI;
using UnityEngine;

namespace Startup.InGame
{
    public class PlanetInitializer : InitializerBase
    {
        [SerializeField] private float _planetZ;
        [SerializeField] private Planet _planetPrefab;
        [SerializeField] private SpriteRenderer _backgroundRenderer;

        private Planet _planet;
        
        public override void Initialize()
        {
            var gameInfo = GameContainer.Common.Resolve<GameInfoContainer>();
            var level = gameInfo.CurrentLevel;
            var mainCamera = Camera.main;

            if (level.OverrideBackgroundImage && level.BackgroundImage != null)
                _backgroundRenderer.sprite = level.BackgroundImage;

            // ReSharper disable once PossibleNullReferenceException
            var bottomLeft = mainCamera.ViewportToWorldPoint(Vector2.zero);
            var topRight = mainCamera.ViewportToWorldPoint(Vector2.one);

            float spawnX = Mathf.Lerp(bottomLeft.x, topRight.x, level.PlanetHorizontalPosition);
            float spawnY = Mathf.Lerp(bottomLeft.y, topRight.y, level.PlanetStartPosition);
            
            float appearTime = level.PlanetLeaveTime;
            float positionDiff = spawnY - bottomLeft.y;
            float speed = positionDiff / appearTime;

            if (level.Planet == null || level.Planet.Texture == null) return;

            _planet = Instantiate(level.Planet.Prefab == null ? _planetPrefab : level.Planet.Prefab);
            _planet.Initialize(level, speed);
            
            _planet.transform.position = new Vector3(
                spawnX,
                spawnY,
                _planetZ);
        }

        public override void Reinitialize()
        {
            if (_planet != null)
                Destroy(_planet.gameObject);
            
            Initialize();
        }
    }
}