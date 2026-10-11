using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Scripts.DI.Services.Global
{
    public interface ICoroutineRunner
    {
        public Coroutine StartCoroutine(IEnumerator routine);

        public void StopCoroutine(Coroutine coroutine);

        public CoroutineQueue CreateCorotineQueue();
    }

    public class ZenjectCoroutineRunner : ICoroutineRunner
    {
        private readonly MonoBehaviour _monoBehaviour;

        public ZenjectCoroutineRunner(MonoBehaviour monoBehaviour)
        {
            _monoBehaviour = monoBehaviour;
        }

        public Coroutine StartCoroutine(IEnumerator routine) => _monoBehaviour.StartCoroutine(routine);

        public void StopCoroutine(Coroutine coroutine) => _monoBehaviour.StopCoroutine(coroutine);

        public CoroutineQueue CreateCorotineQueue()
        {
            CoroutineQueue queue = new(_monoBehaviour);

            return queue;
        }
    }

    public class CoroutineQueue
    {
        private MonoBehaviour _owner = null;
        private Coroutine _internalCoroutine = null;
        private Queue<IEnumerator> _coroutineQueue = new Queue<IEnumerator>();

        public CoroutineQueue(MonoBehaviour coroutineOwner)
        {
            _owner = coroutineOwner;
        }

        public void StartLoop()
        {
            if (_internalCoroutine != null)
                throw new System.InvalidOperationException("Coroutine queue already started");

            _internalCoroutine = _owner.StartCoroutine(Process());
        }

        public void StopLoop()
        {
            if (_internalCoroutine == null)
                throw new System.InvalidOperationException("Coroutine queue not started yet");

            _owner.StopCoroutine(_internalCoroutine);
            _internalCoroutine = null;
        }

        public void EnqueueCoroutine(IEnumerator coroutine)
        {
            _coroutineQueue.Enqueue(coroutine);
        }

        private IEnumerator Process()
        {
            while (true)
            {
                if (_coroutineQueue.Count > 0)
                    yield return _owner.StartCoroutine(_coroutineQueue.Dequeue());
                else
                    yield return null;
            }
        }
    }
}