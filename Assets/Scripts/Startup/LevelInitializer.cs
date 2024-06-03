using Services.DI;
using UnityEngine;

namespace Startup
{
    [DefaultExecutionOrder(-1000)]
    public class LevelInitializer : MonoBehaviour
    {
        private static bool _initialized;
        
        [SerializeField] private InitializerBase[] _initializers;

        private bool _thisInstanceIsInitializer;

        public void Initialize()
        {
            if (_initialized)
            {
                Debug.LogError("Already has initialized level! Destroying initializer...");
                Destroy(gameObject);
                return;
            }
            
            Debug.Log("Initializing game!");
            
            GameContainer.InGame = new Container();
            GameContainer.InGame.Register(this);
            
            foreach (var initializer in _initializers)
            {
                Debug.Log($"Initializing {initializer.GetType().Name}");
                initializer.Initialize();
            }

            _thisInstanceIsInitializer = true;
            _initialized = true;
        }

        public void Reinitialize()
        {
            Debug.Log("Reinitializing game");
            
            foreach (var initializer in _initializers)
            {
                Debug.Log($"Reinitializing {initializer.GetType().Name}");
                initializer.Reinitialize();
            }
        }

        private void OnDestroy()
        {
            if (!_thisInstanceIsInitializer) return;
            
            foreach (var initializer in _initializers)
            {
                initializer.Dispose();
            }
            _initialized = false;
            _thisInstanceIsInitializer = false;
        }
    }
}