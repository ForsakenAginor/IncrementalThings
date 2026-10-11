using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Source.Scripts.Utility.Pools
{
    public class Pool<T> where T : Poolable
    {
        private readonly IPoolableFactory _factory;
        private readonly ObjectPool<T> _pool;
        private readonly T[] _prefabs;
        private readonly Transform _holder;

        public Pool(IPoolableFactory factory, T prefab, Transform holder, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
        {
            _factory = factory != null ? factory : throw new ArgumentNullException(nameof(factory));
            _holder = holder != null ? holder : throw new ArgumentNullException(nameof(holder));
            _prefabs = new T[1] { prefab };

            _pool = new ObjectPool<T>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPooledItem,
                collectionCheck, defaultCapacity, maxSize);
        }

        public Pool(IPoolableFactory factory, T[] prefabs, Transform holder, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
        {
            _factory = factory != null ? factory : throw new ArgumentNullException(nameof(factory));
            _prefabs = prefabs != null ? prefabs : throw new ArgumentNullException(nameof(prefabs));
            _holder = holder != null ? holder : throw new ArgumentNullException(nameof(holder));

            _pool = new ObjectPool<T>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPooledItem,
                collectionCheck, defaultCapacity, maxSize);
        }

        public T Get()
        {
            var item = _pool.Get();
            item.ReadyToRelease += OnReadyToRelease;
            return item;
        }

        private void OnReadyToRelease(Poolable poolable)
        {
            poolable.ReadyToRelease -= OnReadyToRelease;
            _pool.Release((T)poolable);
        }

        private T CreatePooledItem()
        {
            T prefab = _prefabs[UnityEngine.Random.Range(0, _prefabs.Length)];
            T item = (T)_factory.Create(prefab, _holder);
            return item;
        }

        private void OnTakeFromPool(T item)
        {
            item.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(T item)
        {
            item.gameObject.SetActive(false);
        }

        private void OnDestroyPooledItem(T item)
        {
            UnityEngine.Object.Destroy(item.gameObject);
        }
    }
}