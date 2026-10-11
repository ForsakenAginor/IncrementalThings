using System;
using Source.Scripts.Utility.Pools;
using UnityEngine;

namespace Source.Scripts.DI.Services.Game
{
    public interface IPoolFactory
    {
        public Pool<T> CreatePool<T>(T prefab, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000) where T : Poolable;

        public Pool<T> CreatePool<T>(T[] prefabs, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000) where T : Poolable;
    }

    public class PoolFactory : IPoolFactory
    {
        private readonly IPoolableFactory _poolableFactory;
        private readonly Transform _parent;

        public PoolFactory(IPoolableFactory poolableFactory, Transform parent)
        {
            _poolableFactory = poolableFactory != null ? poolableFactory : throw new ArgumentNullException(nameof(poolableFactory));
            _parent = parent != null ? parent : throw new ArgumentNullException(nameof(parent));
        }

        public Pool<T> CreatePool<T>(T prefab, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000) where T : Poolable
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            Transform holder = CreateHolder<T>();

            return new Pool<T>(
                _poolableFactory,
                prefab,
                holder,
                collectionCheck,
                defaultCapacity,
                maxSize
            );
        }

        public Pool<T> CreatePool<T>(T[] prefabs, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000) where T : Poolable
        {
            if (prefabs == null || prefabs.Length == 0)
                throw new ArgumentNullException(nameof(prefabs));

            Transform holder = CreateHolder<T>();

            return new Pool<T>(
                _poolableFactory,
                prefabs,
                holder,
                collectionCheck,
                defaultCapacity,
                maxSize
            );
        }

        private Transform CreateHolder<T>() where T : Poolable
        {
            string holderName = $"{typeof(T).Name}Pool";
            GameObject holderObject = new GameObject(holderName);
            holderObject.transform.SetParent(_parent);
            return holderObject.transform;
        }
    }
}