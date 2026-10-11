using System.Threading;
using UnityEngine;

namespace Source.Scripts.DI.Services.Global
{
    public interface ISceneCancellationTokenProvider
    {
        public CancellationToken Token { get; }
    }

    public interface ITaskFactory
    {
        public TaskQueue CreateTaskQueue();
    }

    public class TaskFactory : MonoBehaviour, ISceneCancellationTokenProvider, ITaskFactory
    {
        public CancellationToken Token => destroyCancellationToken;

        public TaskQueue CreateTaskQueue()
        {
            return new TaskQueue(destroyCancellationToken);
        }
    }
}

