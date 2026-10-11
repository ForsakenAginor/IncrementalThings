using System;
using System.Collections.Generic;

namespace Source.Scripts.Utility
{
    public abstract class State
    {
        private readonly Transition[] _transitions;

        public State(Transition[] transitions)
        {
            _transitions = transitions != null ? transitions : throw new ArgumentNullException(nameof(transitions));
        }

        public event Action BecomeReadyToTransit;

        public IEnumerable<Transition> Transitions => _transitions;

        public abstract void DoThing();

        protected void CallBecomeReadyToTransit()
        {
            BecomeReadyToTransit?.Invoke();
        }
    }

    public abstract class Transition
    {
        private State _targetState;
        private bool _isReadyToTransit;

        public State TargetState => _targetState;

        public bool IsReadyToTransit => _isReadyToTransit;

        public void SetIsReady(bool value)
        {
            _isReadyToTransit = value;
        }

        protected void SetTargetState(State state)
        {
            _targetState = state != null ? state : throw new ArgumentNullException(nameof(state));
        }
    }
}