using UnityEngine;
using Zenject;

namespace Source.Scripts.DI.Services.Global
{
    public interface IZenjectInstantiateWrapper
    {
        public GameObject Instantiate(GameObject original);
        public GameObject Instantiate(GameObject original, Transform parent);
        public GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation);
        public GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation, Transform parent);

        public T Instantiate<T>(T original) where T : Object;
        public T Instantiate<T>(T original, Transform parent) where T : Object;
        public T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object;
        public T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object;

        public T Create<T>() where T : class;
    }


    public class ZenjectInstantiateWrapper : IZenjectInstantiateWrapper
    {
        private readonly DiContainer _container;

        public ZenjectInstantiateWrapper(DiContainer container)
        {
            _container = container;
        }

        public GameObject Instantiate(GameObject original)
        {
            return _container.InstantiatePrefab(original);
        }

        public GameObject Instantiate(GameObject original, Transform parent)
        {
            return _container.InstantiatePrefab(original, parent);
        }

        public GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation)
        {
            return _container.InstantiatePrefab(original, position, rotation, null);
        }

        public GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation, Transform parent)
        {
            return _container.InstantiatePrefab(original, position, rotation, parent);
        }

        public T Instantiate<T>(T original) where T : Object
        {
            return _container.InstantiatePrefabForComponent<T>(original);
        }

        public T Instantiate<T>(T original, Transform parent) where T : Object
        {
            return _container.InstantiatePrefabForComponent<T>(original, parent);
        }

        public T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
        {
            return _container.InstantiatePrefabForComponent<T>(original, position, rotation, null);
        }

        public T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            return _container.InstantiatePrefabForComponent<T>(original, position, rotation, parent);
        }

        public T Create<T>() where T : class
        {
            return _container.Instantiate<T>();
        }
    }

}