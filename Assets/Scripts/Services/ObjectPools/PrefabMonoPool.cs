using System.Collections.Generic;
using UnityEngine;
using Utils;
// ReSharper disable StaticMemberInGenericType

namespace Services.ObjectPools
{
    public class PrefabMonoPool<T> where T : MonoBehaviour
	{
		private static readonly Dictionary<int, int> SpawnedInstancesPrefabIds = new();
		private static readonly Dictionary<int, PrefabMonoPool<T>> PrefabPools = new();

		private static GameObject _mainRoot;
		
		private GameObject _root;
		private readonly T _prefab;
		private readonly Queue<T> _queue;

		private PrefabMonoPool(T prefab, int initialCapacity)
		{
			var type = typeof(T);
			int instanceId = prefab.GetInstanceID();
			_root = new GameObject($"Prefab pool {type.Name} - {instanceId.ToString()}");
			_root.transform.SetParent(_mainRoot.transform);
			_prefab = prefab;
			_queue = new Queue<T>();

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
			
			var type = typeof(T);
			_root = new GameObject($"Prefab pool {type.Name} - {_prefab.GetInstanceID().ToString()}");
			_root.transform.SetParent(_mainRoot.transform);
			_queue.Clear();
		}

		private T Get()
		{
			EnsureCreatedRoot();

			T instance;
			if (_queue.TryDequeue(out var result))
			{
				instance = result;
				if (result == null)
				{
#if UNITY_EDITOR
					Debug.LogError($"Requesting pooled instance of type {typeof(T).Name} NULL!");
#endif
					return null;
				}
			}
			else
			{
				instance = Object.Instantiate(_prefab, _root.transform);
				if (instance == null)
				{
#if UNITY_EDITOR
					Debug.LogError($"Creating pooled instance of type {typeof(T).Name} NULL!");
#endif
					return null;
				}
			}
			
			instance.gameObject.SetActive(true);
			return instance;
		}

		private void Return(T value)
		{
			if (value == null)
			{
#if UNITY_EDITOR
				Debug.LogError($"Return pooled instance of type {typeof(T).Name} NULL!");
#endif
				return;
			}
			value.gameObject.SetActive(false);
			value.transform.SetParent(_root.transform);
			_queue.Enqueue(value);
		}

		public static T GetPrefabInstance(T prefab, int initialCapacity = 0)
		{
			EnsureCreatedMainRoot();
			
			int prefabInstanceId = prefab.GetInstanceID();
			if (!PrefabPools.TryGetValue(prefabInstanceId, out var pool))
			{
				pool = new PrefabMonoPool<T>(prefab, initialCapacity);
				PrefabPools.Add(prefabInstanceId, pool);
			}
			
			var instance = pool.Get();
			if (instance == null) return null;
			
			SpawnedInstancesPrefabIds.Add(instance.GetInstanceID(), prefabInstanceId);
			return instance;
		}

		public static T GetPrefabInstanceForParent(T prefab, Transform parent, int initialCapacity = 0)
		{
			var instance = GetPrefabInstance(prefab, initialCapacity);
			instance.transform.SetParent(parent);
			instance.transform.MoveToLocalZero();
			
			return instance;
		}

		public static void ReturnInstance(T instance)
		{
			if (instance == null)
			{
#if UNITY_EDITOR
				Debug.LogError("Return null instance!!");
#endif
				return;
			}
			
			int instanceId = instance.GetInstanceID();
			if (!SpawnedInstancesPrefabIds.TryGetValue(instanceId, out int prefabId))
			{
				Object.Destroy(instance.gameObject);
				return;
			}

			var pool = PrefabPools[prefabId];
			pool.Return(instance);
			SpawnedInstancesPrefabIds.Remove(instance.GetInstanceID());
		}

		private static void EnsureCreatedMainRoot()
		{
			if (_mainRoot != null) return;

			var type = typeof(T);
			_mainRoot = new GameObject($"Prefab Mono Pools {type.Name}");
			PrefabPools.Clear();
			SpawnedInstancesPrefabIds.Clear();
		}
    }
}