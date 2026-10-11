using System;
using UnityEngine;

namespace Source.Scripts.DI.Services.Game
{
    public interface IGameTimeService
    {
        /// <summary>
        /// first argument time, second fixed delta
        /// </summary>
        public event Action<float, float> TimeChanged;

        public float CurrentLevelTime { get; }
    }

    public class GameTimeService : MonoBehaviour, IGameTimeService
    {
        private float _levelTime = 0;

        public event Action<float, float> TimeChanged;

        public float CurrentLevelTime => _levelTime;

        private void FixedUpdate()
        {
            _levelTime += Time.fixedDeltaTime;
            TimeChanged?.Invoke(_levelTime, Time.fixedDeltaTime);
        }

        public void SetTime()
        {
            _levelTime = 1790f;
        }
    }
}