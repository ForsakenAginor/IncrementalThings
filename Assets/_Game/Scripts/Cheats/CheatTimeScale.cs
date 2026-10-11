using UnityEngine;

namespace Source.Scripts.Cheats
{
    public class CheatTimeScale : MonoBehaviour
    {
        [Header("Time Scale Settings")]
        [SerializeField] private float _normalTimeScale = 1f;
        [SerializeField] private float _slowTimeScale = 0.1f;
        [SerializeField] private float _fastTimeScale = 5f;

        private State _state = State.Normal;

        private void Awake()
        {
            // Initialize time scale to match the initial state
            ApplyTimeScale();
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                SetState(State.Normal);
            }
            else if (Input.GetKeyDown(KeyCode.F7))
            {
                SetState(State.Slow);
            }
            else if (Input.GetKeyDown(KeyCode.F8))
            {
                SetState(State.Fast);
            }
            else if (Input.GetKeyDown(KeyCode.F5))
            {
                PausePlayMode();
            }
        }

        private void PausePlayMode()
        {
#if UNITY_EDITOR
            // Toggle Unity Editor Play Mode pause
            UnityEditor.EditorApplication.isPaused = !UnityEditor.EditorApplication.isPaused;
            Debug.Log($"Editor Play Mode {(UnityEditor.EditorApplication.isPaused ? "paused" : "resumed")}");
#else
            // In build, fallback to time scale control
            if (Time.timeScale == 0f)
            {
                // If currently paused, resume to normal time scale
                Time.timeScale = _normalTimeScale;
                Debug.Log("Resume: TimeScale = " + Time.timeScale);
            }
            else
            {
                // If currently playing, pause the game
                Time.timeScale = 0f;
                Debug.Log("Paused: TimeScale = " + Time.timeScale);
            }
#endif
        }

        private void SetState(State newState)
        {
            if (_state == newState)
                return;

            _state = newState;
            ApplyTimeScale();
            Debug.Log($"TimeScale changed to: {_state}");
        }

        private void ApplyTimeScale()
        {
            switch (_state)
            {
                case State.Normal:
                    Time.timeScale = _normalTimeScale;
                    break;
                case State.Slow:
                    Time.timeScale = _slowTimeScale;
                    break;
                case State.Fast:
                    Time.timeScale = _fastTimeScale;
                    break;
            }
        }

        private enum State
        {
            Normal,
            Slow,
            Fast
        }
    }
}