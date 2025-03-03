﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelObjects
{
    public class ObjectsSpawnerService : MonoBehaviour
    {
        [Serializable]
        private class SpawnerContainer
        {
            [SerializeField] public LevelObjectType Type;
            [SerializeField] public ObjectsSpawnerBase Spawner;
        }

        [SerializeField] private List<SpawnerContainer> _spawners;

        public void Initialize()
        {
            foreach (var container in _spawners)
            {
                Debug.Log($"Initializing spawner {container.Spawner.gameObject.GetInstanceID()} of type {container.Type}");
                container.Spawner.Initialize();
            }
        }

        public ObjectsSpawnerBase GetSpawnerForType(LevelObjectType type)
        {
            foreach (var spawner in _spawners)
            {
                if (spawner.Type == type) return spawner.Spawner;
            }

            return null;
        }
    }
}