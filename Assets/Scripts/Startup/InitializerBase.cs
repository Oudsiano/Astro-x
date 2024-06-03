using UnityEngine;

namespace Startup
{
    public abstract class InitializerBase : MonoBehaviour
    {
        public abstract void Initialize();
        public virtual void Dispose() { }
        public virtual void Reinitialize() { }
    }
}