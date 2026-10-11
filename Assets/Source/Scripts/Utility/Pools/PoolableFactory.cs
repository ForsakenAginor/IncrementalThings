using UnityEngine;
using Zenject;

namespace Source.Scripts.Utility.Pools
{
    public interface IPoolableFactory
    {
        public Poolable Create(Poolable prefab, Transform parent);
    }

    public class PoolableFactory : IPoolableFactory
    {
        private readonly DiContainer _container;

        public PoolableFactory(DiContainer container)
        {
            _container = container;
        }

        public Poolable Create(Poolable prefab, Transform parent)
        {
            return _container.InstantiatePrefab(prefab, parent).GetComponent<Poolable>();
        }
    }
}