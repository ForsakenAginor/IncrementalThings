using System;

namespace ScriptGoblin.NumberForge
{
    /// <summary>
    /// Low-level utilities for <see cref="BigNumber"/> parsing and normalization.
    /// Most callers should use <see cref="BigNumber"/> directly; these methods are
    /// exposed for advanced use cases such as custom serialization.
    /// </summary>
    public static class BigNumberUtils
    {
        /// <summary>
        /// Normalizes a raw mantissa/exponent pair so that the mantissa is in the
        /// range [1, 10) for positive values, or (-10, -1] for negative values.
        /// </summary>
        /// <param name="mantissa">The raw significand.</param>
        /// <param name="exponent">The raw base-10 exponent.</param>
        /// <returns>A normalized (mantissa, exponent) pair.</returns>
        public static (double mantissa, long exponent) Normalize(double mantissa, long exponent)
        {
            if (mantissa == 0) return (0, 0);

            var shift = (long)Math.Floor(Math.Log10(Math.Abs(mantissa)));
            mantissa /= Math.Pow(10, shift);
            exponent += shift;

            if (Math.Abs(mantissa) >= 10) { mantissa /= 10; exponent++; }
            else if (Math.Abs(mantissa) < 1) { mantissa *= 10; exponent--; }

            return (mantissa, exponent);
        }

        /// <summary>
        /// Attempts to parse a string into a <see cref="BigNumber"/> without throwing.
        /// Delegates to <see cref="BigNumber.TryParse"/>.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="result">The parsed value, or <see cref="BigNumber.Zero"/> on failure.</param>
        /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
        public static bool TryParse(string value, out BigNumber result) => BigNumber.TryParse(value, out result);

        /// <summary>
        /// Parses a string into a raw (mantissa, exponent) pair without normalizing.
        /// Supports plain integers (<c>"1000000"</c>), decimals (<c>"1.5"</c>),
        /// and scientific notation (<c>"1.5e9"</c>).
        /// The caller is responsible for passing the result to <see cref="Normalize"/>.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <returns>An un-normalized (mantissa, exponent) pair.</returns>
        public static (double mantissa, long exponent) Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return (0, 0);

            value = value.Trim().Replace(",", "").Replace(" ", "").ToLowerInvariant();

            var eIndex = value.IndexOf('e');
            if (eIndex >= 0)
            {
                if (!double.TryParse(value[..eIndex], out var mantissa) ||
                    !long.TryParse(value[(eIndex + 1)..], out var exponent))
                    throw new FormatException($"Cannot parse '{value}' as a BigNumber.");
                return (mantissa, exponent);
            }

            if (value.IndexOf('.') >= 0)
            {
                value = value.TrimEnd('0');
                if (!double.TryParse(value, out var asDouble))
                    throw new FormatException($"Cannot parse '{value}' as a BigNumber.");
                if (asDouble == 0) return (0, 0);
                var exponent = (int)Math.Floor(Math.Log10(Math.Abs(asDouble)));
                return (asDouble / Math.Pow(10, exponent), exponent);
            }

            return ParseIntegerString(value);
        }

        /// <summary>
        /// Parses a plain integer string (no decimal point, no exponent notation)
        /// into a raw (mantissa, exponent) pair. Handles arbitrarily long digit strings
        /// by retaining up to 15 significant digits and folding excess digits into
        /// the exponent.
        /// </summary>
        /// <param name="value">A string of digits, optionally prefixed with <c>'-'</c>.</param>
        /// <returns>An un-normalized (mantissa, exponent) pair.</returns>
        public static (double mantissa, long exponent) ParseIntegerString(string value)
        {
            if (value == "0") return (0, 0);

            var negative = value[0] == '-';
            if (negative) value = value[1..];
            value = value.TrimStart('0');

            var length      = value.Length;
            var mantissaStr = value[..Math.Min(15, length)];
            if (!double.TryParse(mantissaStr, out var mantissa))
                throw new FormatException($"Cannot parse '{value}' as a BigNumber.");
            long exponent   = length - mantissaStr.Length;

            return (negative ? -mantissa : mantissa, exponent);
        }
    }
}
