using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Services.ObjectPools
{
    public class PrefabGameObjectPool
    {
        private static readonly Dictionary<int, int> _spawnedInstancesPrefabIds = new();
		private static readonly Dictionary<int, PrefabGameObjectPool> _prefabPools = new();

		private static GameObject _mainRoot;

		private GameObject _root;
		private readonly GameObject _prefab;
		private readonly Queue<GameObject> _queue;

		private PrefabGameObjectPool(GameObject prefab, int initialCapacity)
		{
			int instanceId = prefab.GetInstanceID();
			_root = new GameObject($"Prefab pool {prefab.name} - {instanceId.ToString()}");
			_root.transform.SetParent(_mainRoot.transform);
			_prefab = prefab;
			_queue = new Queue<GameObject>();

			for (int i = 0; i < initialCapacity; i++)
			{
				var instance = Object.Instantiate(prefab, _root.transform);
				instance.gameObject.SetActive(false);
				_queue.Enqueue(instance);
			}
		}

		private void EnsureCreatedRoot()
		{
			if (_root != null) return;
			
			_root = new GameObject($"Prefab pool {_prefab.name} - {_prefab.GetInstanceID().ToString()}");
			_root.transform.SetParent(_mainRoot.transform);
			_queue.Clear();
		}

		private GameObject Get()
		{
			EnsureCreatedRoot();
			
			var instance = _queue.TryDequeue(out var result) ? result : Object.Instantiate(_prefab, _root.transform);
			if (instance == null)
			{
				Debug.LogError($">>> Requesting pooled instance of prefab {_prefab.name} NULL!");
				return null;
			}
			
			instance.gameObject.SetActive(true);
			return instance;
		}

		private void Return(GameObject value)
		{
			value.gameObject.SetActive(false);
			value.transform.SetParent(_root.transform);
			_queue.Enqueue(value);
		}

		public static GameObject GetPrefabInstance(GameObject prefab, int initialCapacity = 0)
		{
			EnsureCreatedMainRoot();
			
			int prefabInstanceId = prefab.GetInstanceID();
			if (!_prefabPools.TryGetValue(prefabInstanceId, out var pool))
			{
				pool = new PrefabGameObjectPool(prefab, initialCapacity);
				_prefabPools.Add(prefabInstanceId, pool);
			}
			
			var instance = pool.Get();
			_spawnedInstancesPrefabIds.Add(instance.GetInstanceID(), prefabInstanceId);
			return instance;
		}

		public static GameObject GetPrefabInstanceForParent(GameObject prefab, Transform parent, int initialCapacity = 0)
		{
			var instance = GetPrefabInstance(prefab, initialCapacity);
			instance.transform.SetParent(parent);
			instance.transform.MoveToLocalZero();
			
			return instance;
		}

		public static void ReturnInstance(GameObject instance)
		{
			int instanceId = instance.GetInstanceID();
			if (!_spawnedInstancesPrefabIds.TryGetValue(instanceId, out int prefabId))
			{
				Object.Destroy(instance.gameObject);
				return;
			}

			var pool = _prefabPools[prefabId];
			pool.Return(instance);
			_spawnedInstancesPrefabIds.Remove(instance.GetInstanceID());
		}

		private static void EnsureCreatedMainRoot()
		{
			if (_mainRoot != null) return;

			_mainRoot = new GameObject("Prefab Game Objects Pools");
			_prefabPools.Clear();
			_spawnedInstancesPrefabIds.Clear();
		}
    }
}