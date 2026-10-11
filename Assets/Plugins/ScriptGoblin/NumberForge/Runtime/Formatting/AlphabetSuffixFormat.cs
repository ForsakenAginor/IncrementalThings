using System;
using System.Text;

namespace ScriptGoblin.NumberForge.Formatting
{
    /// <summary>
    /// Форматирует <see cref="BigNumber"/> в стиле "1.123a", "1.123z", "1.123aa", "1.123zz".
    /// </summary>
    /// <remarks>
    /// <para>
    /// Уровень суффикса соответствует <c>Exponent / 3</c> (1-based).
    /// Уровень 1..26 → 'a'..'z'; 27..702 → 'aa'..'zz'; 703..18278 → 'aaa'..'zzz'; и т.д.
    /// </para>
    /// <para>
    /// Значения с <c>Exponent &lt; 3</c> выводятся без суффикса (как обычные числа).
    /// Отрицательные экспоненты выводятся в научной нотации <c>1.5e-3</c>.
    /// </para>
    /// </remarks>
    public class AlphabetSuffixFormat : IBigNumberFormatter
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        /// <inheritdoc/>
        public string Format(BigNumber value, FormatOptions options = null)
        {
            options ??= new FormatOptions();

            // Ноль
            if (value.Mantissa == 0) return options.FormatMantissa(0);

            // Отрицательная экспонента — научная нотация
            if (value.Exponent < 0)
                return $"{options.FormatMantissa(value.Mantissa)}e{value.Exponent}";

            // До 1000 — без суффикса
            if (value.Exponent < 3)
            {
                var plain = value.Mantissa * Math.Pow(10, value.Exponent);
                return options.FormatMantissa(plain);
            }

            // level: 1 = 'a', 26 = 'z', 27 = 'aa', 702 = 'zz', ...
            long level  = value.Exponent / 3;
            int  remainder = (int)(value.Exponent % 3);

            // Мантисса в диапазоне [1, 1000), как в инженерной нотации
            double mantissa = value.Mantissa * Math.Pow(10, remainder);

            string suffix = LevelToSuffix(level);
            return options.FormatMantissa(mantissa) + suffix;
        }

        /// <summary>
        /// Преобразует номер уровня (1-based) в буквенный суффикс.
        /// 1 → "a", 26 → "z", 27 → "aa", 702 → "zz", 703 → "aaa", ...
        /// </summary>
        private static string LevelToSuffix(long level)
        {
            if (level <= 0) return string.Empty;

            // Определяем количество букв в суффиксе.
            // Сумма 26^1 + 26^2 + ... + 26^k >= level.
            // k = 1 покрывает 1..26, k = 2 покрывает 27..702, и т.д.
            int length = 1;
            long cumulative = 26;
            while (level > cumulative)
            {
                length++;
                // 26^length может переполнить long при length > 13,
                // но до этого уровня Exponent всё равно не дойдёт (long.MaxValue / 3 ≈ 3e18,
                // а 26^14 ≈ 6.4e19 — уже за пределами).
                cumulative += Pow26(length);
            }

            // Смещение внутри блока данной длины
            long offsetInBlock = level - (cumulative - Pow26(length)) - 1; // 0-based

            var sb = new StringBuilder(length);
            for (int i = length - 1; i >= 0; i--)
            {
                long digit = offsetInBlock % 26;
                offsetInBlock /= 26;
                sb.Insert(0, Alphabet[(int)digit]);
            }
            return sb.ToString();
        }

        private static long Pow26(int power)
        {
            long result = 1;
            for (int i = 0; i < power; i++) result *= 26;
            return result;
        }
    }
}