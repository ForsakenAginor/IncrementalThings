using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Source.Scripts.Utility.Configuration
{
    public abstract class SerializedConfiguration<T1, T2> : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<T1, T2> _data = new Dictionary<T1, T2>();

        protected IReadOnlyDictionary<T1, T2> Data => _data;

        public T2 GetObjectByKey(T1 key)
        {
            if (_data.ContainsKey(key))
                return _data[key];
        
            throw new KeyNotFoundException($"{ToString()}: Key not found in configuration: {key.ToString()}");
        }
    }
}