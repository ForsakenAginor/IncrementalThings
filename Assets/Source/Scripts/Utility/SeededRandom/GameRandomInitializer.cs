using System;
using System.Reflection;

namespace Source.Scripts.Utility.SeededRandom
{
    public interface IGameRandomInitializer
    {
        /// <summary>
        /// Gets the seed currently held by this initializer.
        /// </summary>
        public int Seed { get; }

        /// <summary>
        /// Sets a new seed and recreates the underlying <see cref="SeededRandom"/> state.
        /// Any previous random sequence is discarded.
        /// </summary>
        /// <param name="seed">The seed value to apply.</param>
        public void SetSeed(int seed);
    }
    
    public sealed class GameRandomInitializer : IGameRandomInitializer
    {
        private static readonly ConstructorInfo Ctor;

        private int _seed;

        static GameRandomInitializer()
        {
            Ctor = typeof(SeededRandom).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                types: new[] { typeof(int) },
                modifiers: null);

            if (Ctor == null)
                throw new InvalidOperationException(
                    "SeededRandom(int) private constructor was not found. " +
                    "Make sure it exists and is preserved by the IL2CPP linker.");
        }

        public GameRandomInitializer()
        {
            SetSeed(0);
        }

        /// <inheritdoc />
        public int Seed => _seed;

        /// <inheritdoc />
        public void SetSeed(int seed)
        {
            _seed = seed;
            Ctor.Invoke(new object[] { seed });
        }
    }
}