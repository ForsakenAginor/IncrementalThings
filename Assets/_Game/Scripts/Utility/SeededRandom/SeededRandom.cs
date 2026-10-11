using System;
using System.Collections.Generic;

namespace Source.Scripts.Utility.SeededRandom
{
    public sealed class SeededRandom
    {
        private static System.Random _random;
        private static int _seed;

        // Private on purpose. Only GameRandomInitializer is expected to reach it (via reflection).
        // [Preserve] keeps it from being stripped by the IL2CPP managed code linker.
        [UnityEngine.Scripting.Preserve]
        private SeededRandom(int seed)
        {
            _seed = seed;
            _random = new System.Random(seed);
        }
        
        /// <summary>
        /// Returns a random integer in the range [<paramref name="minInclusive"/>, <paramref name="maxExclusive"/>).
        /// </summary>
        /// <param name="minInclusive">Inclusive lower bound.</param>
        /// <param name="maxExclusive">Exclusive upper bound. Must be greater than <paramref name="minInclusive"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="minInclusive"/> is greater than or equal to <paramref name="maxExclusive"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the generator has not been initialized.</exception>
        public static int Range(int minInclusive, int maxExclusive)
        {
            if (minInclusive >= maxExclusive)
                throw new ArgumentException("minInclusive must be less than maxExclusive.");

            return Rng.Next(minInclusive, maxExclusive);
        }

        /// <summary>
        /// Returns a random float in the range [<paramref name="min"/>, <paramref name="max"/>).
        /// </summary>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Inclusive upper bound.</param>
        /// <exception cref="InvalidOperationException">Thrown when the generator has not been initialized.</exception>
        public static float Range(float min, float max)
        {
            return min + (float)Rng.NextDouble() * (max - min);
        }

        /// <summary>
        /// Returns <c>true</c> with the given probability.
        /// </summary>
        /// <param name="probability">Probability of returning <c>true</c>. Expected in [0.0, 1.0].</param>
        /// <exception cref="InvalidOperationException">Thrown when the generator has not been initialized.</exception>
        public static bool Chance(float probability)
        {
            return Rng.NextDouble() < probability;
        }

        /// <summary>
        /// Returns a random element from the list.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="list">Source list. Must be non-null and non-empty.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="list"/> is empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the generator has not been initialized.</exception>
        public static T Pick<T>(IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (list.Count == 0)
                throw new ArgumentException("List must not be empty.", nameof(list));

            return list[Rng.Next(list.Count)];
        }

        /// <summary>
        /// Shuffles the list in place using the Fisher-Yates algorithm.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="list">List to shuffle. Must be non-null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the generator has not been initialized.</exception>
        public static void Shuffle<T>(IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        /// <summary>
        /// Shuffles the array in place using the Fisher-Yates algorithm.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="array">Array to shuffle. Must be non-null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the generator has not been initialized.</exception>
        public static void Shuffle<T>(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Rng.Next(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        // Single access point that guards against use-before-initialization.
        private static System.Random Rng
        {
            get
            {
                if (_random == null)
                    throw new InvalidOperationException(
                        "SeededRandom has not been initialized. Use IGameRandomInitializer.SetSeed first.");
                
                return _random;
            }
        }
    }
}