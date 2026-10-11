using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Source.Scripts.DI.Services.Global
{
    public class TaskQueue
    {
        private readonly CancellationToken _globalCancellationToken;
        
        private readonly Queue<Func<CancellationToken, UniTask>> _queue = new();
        private CancellationTokenSource _cts;

        public TaskQueue(CancellationToken globalToken)
        {
            _globalCancellationToken = globalToken;
        }

        public void StartLoop()
        {
            if (_cts != null)
                throw new InvalidOperationException("Queue already started");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(_globalCancellationToken);
            ProcessAsync(_cts.Token);
        }

        public void StartLoop(CancellationToken externalToken)
        {
            if (_cts != null)
                throw new InvalidOperationException("Queue already started");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken, _globalCancellationToken);
            ProcessAsync(_cts.Token);
        }

        public void StopLoop()
        {
            if (_cts == null)
                throw new InvalidOperationException("Queue not started yet");

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        public void Enqueue(Func<CancellationToken, UniTask> taskFactory)
        {
            _queue.Enqueue(taskFactory);
        }

        private async UniTaskVoid ProcessAsync(CancellationToken token)
        {
            while (true)
            {
                if (_queue.Count > 0)
                {
                    var factory = _queue.Dequeue();
                    await factory(token);
                }
                else
                {
                    await UniTask.NextFrame(token);
                }
            }
        }
    }
}