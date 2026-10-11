using System;
using R3;

namespace Source.Scripts.Utility
{
    public abstract class Resource : IResource
    {
        private int _maximum;
        private ReactiveProperty<int> _amount;

        /// <summary>
        /// Create Resource with "Maximum" equal to starting "amount" value
        /// </summary>
        /// <param name="amount"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Resource(int amount)
        {
            _amount = amount >= 0 ? new(amount) : throw new ArgumentOutOfRangeException(nameof(amount));
            _maximum = amount;
        }
        
        public Resource(int amount, int maximum = int.MaxValue)
        {
            _amount = amount >= 0 ? new() : throw new ArgumentOutOfRangeException(nameof(amount));
            _maximum = maximum >= amount ? maximum : throw new ArgumentOutOfRangeException(nameof(maximum));
        }

        public Observable<int> ResourceAmount => _amount;

        public int Maximum => _maximum;

        public void Add(int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            int temp = _amount.Value + amount;
            _amount.Value = Math.Min(_maximum, temp);
        }

        public bool TrySpent(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (_amount.Value < amount)
                return false;

            _amount.Value -= amount;
            return true;
        }

        public void Spent(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            int spent = Math.Min(_amount.Value, amount);
            _amount.Value -= spent;

            if (_amount.Value == 0)
            {
                DoOnResourceOver();
            }
        }

        protected void CallResourceOver()
        {
            _amount.OnCompleted();
        }

        protected abstract void DoOnResourceOver();
    }
}