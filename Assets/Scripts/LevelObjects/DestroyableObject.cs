using Damage;
using DI;
using Imports.BreakableAsteroids.Scripts;
using Services.DI;
using Services.SoundsSystem;
using UnityEngine;

namespace LevelObjects
{
    public class DestroyableObject : MonoBehaviour
    {
        [SerializeField] private Damageable _damageable;
        [SerializeField] private Fracture _fracture;

        [Inject] private SoundsSystem _soundsSystem;
        
        private void Awake()
        {
            GameContainer.InjectToInstance(this);
            _damageable.OnDestroyed += OnDestroyed;
        }

        private void OnDestroyed(DamageType type)
        {
            _soundsSystem.PlaySound(SoundType.RockDestroyed);
            
            if (_fracture != null)
                _fracture.FractureObject();
        }
    }
}