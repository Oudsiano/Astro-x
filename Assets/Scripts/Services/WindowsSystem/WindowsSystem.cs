﻿using System;
using System.Collections.Generic;
using DI;
using Services.DI;
using Ui;
using Object = UnityEngine.Object;

namespace Services.WindowsSystem
{
    public class WindowsSystem
    {
        private readonly UiRoot _uiRoot;

        private readonly Dictionary<Type, WindowBase> _windowsPrefabs;
        private readonly Dictionary<Type, WindowBase> _loadedWindows;

        [Construct]
        public WindowsSystem(GameWindows gameWindows, UiRoot uiRoot)
        {
            _uiRoot = uiRoot;
            _windowsPrefabs = new Dictionary<Type, WindowBase>();
            _loadedWindows = new Dictionary<Type, WindowBase>();

            if (gameWindows.Windows == null)
                return;

            foreach (var window in gameWindows.Windows)
            {
                _windowsPrefabs.Add(window.GetType(), window);
            }
        }

        public void AddWindow<T>(T window) where T : WindowBase
        {
            var type = typeof(T);
            if (_loadedWindows.ContainsKey(type))
                throw new ArgumentException(
                    $"Trying to add window of type {type.Name}, but this window already created!");
            
            _loadedWindows.Add(type, window);
        }

        public bool TryGetWindow<T>(out T window) where T : WindowBase
        {
            window = null;
            var type = typeof(T);
            if (_loadedWindows.TryGetValue(type, out var baseWindow))
            {
                if (baseWindow is not T targetWindow)
                    throw new ArgumentException($"Error in getting window type {type.Name} - have cached wrong type of window");

                window = targetWindow;
                return true;
            }

            return false;
        }

        public T CreateWindow<T>() where T : WindowBase
        {
            var type = typeof(T);
            if (_loadedWindows.TryGetValue(type, out var baseWindow))
            {
                if (baseWindow is not T targetWindow)
                    throw new ArgumentException($"Error in creating window type {type.Name} - already created wrong type of window");

                return targetWindow;
            }
            
            if (!_windowsPrefabs.TryGetValue(type, out var windowPrefabBase))
                throw new ArgumentException($"Error in getting window type {type.Name} - window not registered");
            
            if (windowPrefabBase is not T windowPrefab)
                throw new ArgumentException($"Error in getting window type {type.Name} - registered wrong window");
            
            var window = GameContainer.InstantiateAndResolve(windowPrefab, _uiRoot.WindowsParent);
            _loadedWindows.Add(type, window);
            return window;
        }

        public void DestroyWindow<T>() where T : WindowBase
        {
            var type = typeof(T);
            if (!_loadedWindows.TryGetValue(type, out var window))
                return;
            
            if (window != null && window.gameObject != null)
                Object.Destroy(window.gameObject);

            _loadedWindows.Remove(type);
        }

        public void DestroyWindow<T>(T windowObject) where T : WindowBase
        {
            var type = typeof(T);
            if (!_loadedWindows.TryGetValue(type, out var window))
                return;
            
            if (window != null && window.gameObject != null)
                Object.Destroy(window.gameObject);

            _loadedWindows.Remove(type);
        }

        public void DestroyAll()
        {
            foreach (var loadedWindow in _loadedWindows)
            {
                if (loadedWindow.Value != null && loadedWindow.Value.gameObject != null)
                    Object.Destroy(loadedWindow.Value.gameObject);
            }
            
            _loadedWindows.Clear();
        }
    }
}