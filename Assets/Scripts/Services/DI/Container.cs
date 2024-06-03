using System;
using System.Collections.Generic;

namespace Services.DI
{
    public sealed class Container
    {
        private readonly Dictionary<Type, object> _registrations = new();

        public void Register<T>(T value, bool overrideExisting = false)
        {
            var type = typeof(T);
            if (_registrations.ContainsKey(type) && !overrideExisting)
                throw new ArgumentException("Trying to register already registered type!");
            
            _registrations[type] = value;
        }

        // Methods for direct usage
        public T Resolve<T>() => _registrations.TryGetValue(typeof(T), out object value) ? (T)value : default;
        public bool CanResolve<T>() => _registrations.ContainsKey(typeof(T));

        // Methods for automatic injection
        public object Resolve(Type type) => _registrations.GetValueOrDefault(type);
        public bool CanResolve(Type type) => _registrations.ContainsKey(type);

        public void Dispose()
        {
            foreach (var registration in _registrations)
            {
                if (registration.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            
            _registrations.Clear();
        }
    }
}