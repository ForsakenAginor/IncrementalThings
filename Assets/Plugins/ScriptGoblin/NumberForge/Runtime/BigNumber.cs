#nullable enable
using System;
using ScriptGoblin.NumberForge.Formatting;

namespace ScriptGoblin.NumberForge
{
    /// <summary>
    /// Represents an arbitrarily large positive or negative number using a mantissa/exponent
    /// representation: <c>value = Mantissa × 10^Exponent</c>.
    /// The mantissa is always kept in the range [1, 10) or (-10, -1], so each value has a
    /// unique normalized form. Suitable for idle-game currencies, damage numbers, and any
    /// scenario where values routinely exceed <see cref="double"/> or <see cref="long"/> range.
    /// </summary>
    /// <remarks>
    /// <para><b>Precision:</b> the mantissa is a <see cref="double"/>, giving ~15 significant
    /// decimal digits of precision regardless of magnitude.</para>
    /// <para><b>Serialization:</b> Unity's built-in serializer does not support <see cref="double"/>
    /// or <see cref="long"/> fields on structs. To persist a <see cref="BigNumber"/> in a
    /// <see cref="UnityEngine.MonoBehaviour"/> or <see cref="UnityEngine.ScriptableObject"/>,
    /// serialize it as a string and round-trip through <see cref="TryParse"/>.</para>
    /// </remarks>
    [Serializable]
    public readonly struct BigNumber : IComparable<BigNumber>, IEquatable<BigNumber>, IFormattable
    {
        /// <summary>The significand. Always in [1, 10) for positive values, (-10, -1] for negative.</summary>
        public double Mantissa { get; }

        /// <summary>The base-10 exponent. The represented value is <c>Mantissa × 10^Exponent</c>.</summary>
        public long Exponent { get; }

        /// <summary>Creates a <see cref="BigNumber"/> from a mantissa and exponent, normalizing the result.</summary>
        /// <param name="mantissa">The significand. Does not need to be pre-normalized.</param>
        /// <param name="exponent">The base-10 exponent.</param>
        public BigNumber(double mantissa, long exponent)
        {
            (Mantissa, Exponent) = BigNumberUtils.Normalize(mantissa, exponent);
        }

        /// <summary>
        /// Parses a string into a <see cref="BigNumber"/>.
        /// Supports plain integers (<c>"1000000"</c>), decimals (<c>"1.5"</c>),
        /// and scientific notation (<c>"1.5e9"</c>).
        /// </summary>
        /// <param name="number">The string to parse.</param>
        /// <exception cref="FormatException">Thrown if the string cannot be parsed.</exception>
        public BigNumber(string number)
        {
            var (mantissa, exponent) = BigNumberUtils.Parse(number);
            (Mantissa, Exponent) = BigNumberUtils.Normalize(mantissa, exponent);
        }

        /// <summary>The value zero.</summary>
        public static BigNumber Zero => new(0, 0);

        /// <summary>The value one.</summary>
        public static BigNumber One => new(1, 0);

        /// <summary>
        /// A very small positive value, useful as a near-zero threshold in comparisons
        /// and to avoid division-by-zero guards.
        /// </summary>
        public static BigNumber Epsilon => new(1, -15);

        /// <summary>Returns <c>true</c> if this value is exactly zero.</summary>
        public bool IsZero => Mantissa == 0;

        /// <summary>Returns <c>true</c> if this value is greater than zero.</summary>
        public bool IsPositive => Mantissa > 0;

        /// <summary>Returns <c>true</c> if this value is less than zero.</summary>
        public bool IsNegative => Mantissa < 0;

        /// <summary>Implicitly converts an <see cref="int"/> to a <see cref="BigNumber"/>.</summary>
        public static implicit operator BigNumber(int value)    => new(value, 0);

        /// <summary>Implicitly converts a <see cref="long"/> to a <see cref="BigNumber"/>.</summary>
        public static implicit operator BigNumber(long value)   => new(value, 0);

        /// <summary>Implicitly converts a <see cref="double"/> to a <see cref="BigNumber"/>.</summary>
        public static implicit operator BigNumber(double value) => value == 0 ? Zero : new(value, 0);

        /// <summary>Returns the negation of <paramref name="value"/>.</summary>
        public static BigNumber operator -(BigNumber value) => value.IsZero ? Zero : new BigNumber(-value.Mantissa, value.Exponent);

        /// <summary>
        /// Attempts to parse a string into a <see cref="BigNumber"/> without throwing.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="result">
        /// When this method returns <c>true</c>, contains the parsed value;
        /// otherwise <see cref="Zero"/>.
        /// </param>
        /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
        public static bool TryParse(string value, out BigNumber result)
        {
            try   { result = new BigNumber(value); return true; }
            catch { result = Zero; return false; }
        }

        /// <summary>
        /// Converts this <see cref="BigNumber"/> to a <see cref="double"/>.
        /// Returns <see cref="double.PositiveInfinity"/> or <see cref="double.NegativeInfinity"/>
        /// if the magnitude exceeds <see cref="double"/> range.
        /// </summary>
        public double ToDouble() => Mantissa * Math.Pow(10, Exponent);

        /// <summary>
        /// Converts this <see cref="BigNumber"/> to a <see cref="long"/>.
        /// </summary>
        /// <exception cref="OverflowException">Thrown if the value exceeds <see cref="long.MaxValue"/>.</exception>
        public long ToLong()
        {
            if (Exponent > 18) throw new OverflowException("BigNumber is too large to convert to long.");
            return (long)(Mantissa * Math.Pow(10, Exponent));
        }

        /// <summary>Returns the absolute value of this <see cref="BigNumber"/>.</summary>
        public BigNumber Abs() => Mantissa < 0 ? new BigNumber(-Mantissa, Exponent) : this;

        /// <summary>Returns the smaller of two <see cref="BigNumber"/> values.</summary>
        public static BigNumber Min(BigNumber a, BigNumber b) => a <= b ? a : b;

        /// <summary>Returns the larger of two <see cref="BigNumber"/> values.</summary>
        public static BigNumber Max(BigNumber a, BigNumber b) => a >= b ? a : b;

        /// <summary>Clamps <paramref name="value"/> to the range [<paramref name="min"/>, <paramref name="max"/>].</summary>
        public static BigNumber Clamp(BigNumber value, BigNumber min, BigNumber max) => Max(min, Min(max, value));

        /// <summary>
        /// Returns the largest integer less than or equal to this value.
        /// Values with <c>Exponent >= 15</c> are already integral and returned unchanged.
        /// </summary>
        public BigNumber Floor()
        {
            if (Exponent >= 15) return this;
            if (Exponent < 0)   return Mantissa >= 0 ? Zero : new BigNumber(-1, 0);
            return (BigNumber)Math.Floor(Mantissa * Math.Pow(10, Exponent));
        }

        /// <summary>
        /// Returns the smallest integer greater than or equal to this value.
        /// Values with <c>Exponent >= 15</c> are already integral and returned unchanged.
        /// </summary>
        public BigNumber Ceil()
        {
            if (Exponent >= 15) return this;
            if (Exponent < 0)   return Mantissa > 0 ? One : Zero;
            return (BigNumber)Math.Ceiling(Mantissa * Math.Pow(10, Exponent));
        }

        /// <summary>
        /// Linearly interpolates between <paramref name="a"/> and <paramref name="b"/> by <paramref name="t"/>.
        /// <paramref name="t"/> is clamped to [0, 1].
        /// </summary>
        /// <param name="a">The start value (returned when <paramref name="t"/> = 0).</param>
        /// <param name="b">The end value (returned when <paramref name="t"/> = 1).</param>
        /// <param name="t">The interpolation factor, clamped to [0, 1].</param>
        public static BigNumber Lerp(BigNumber a, BigNumber b, double t)
        {
            t = Math.Clamp(t, 0.0, 1.0);
            return a + (b - a) * t;
        }

        /// <summary>Adds two <see cref="BigNumber"/> values.</summary>
        public static BigNumber operator +(BigNumber a, BigNumber b)
        {
            if (a.Mantissa == 0) return b;
            if (b.Mantissa == 0) return a;
            if (a.Exponent < b.Exponent) (a, b) = (b, a);
            var diff = a.Exponent - b.Exponent;
            if (diff > 15) return a;
            return new BigNumber(a.Mantissa + b.Mantissa * Math.Pow(10, -diff), a.Exponent);
        }

        /// <summary>Subtracts <paramref name="b"/> from <paramref name="a"/>.</summary>
        public static BigNumber operator -(BigNumber a, BigNumber b)
        {
            if (b.Mantissa == 0) return a;
            if (a.Mantissa == 0) return new BigNumber(-b.Mantissa, b.Exponent);
            var negate = false;
            if (a.Exponent < b.Exponent) { (a, b) = (b, a); negate = true; }
            var diff = a.Exponent - b.Exponent;
            if (diff > 15) return negate ? new BigNumber(-a.Mantissa, a.Exponent) : a;
            var mantissa = a.Mantissa - b.Mantissa * Math.Pow(10, -diff);
            return new BigNumber(negate ? -mantissa : mantissa, a.Exponent);
        }

        /// <summary>Multiplies two <see cref="BigNumber"/> values.</summary>
        public static BigNumber operator *(BigNumber a, BigNumber b)
        {
            if (a.Mantissa == 0 || b.Mantissa == 0) return Zero;
            return new BigNumber(a.Mantissa * b.Mantissa, a.Exponent + b.Exponent);
        }

        /// <summary>Divides <paramref name="a"/> by <paramref name="b"/>.</summary>
        /// <exception cref="DivideByZeroException">Thrown when <paramref name="b"/> is zero.</exception>
        public static BigNumber operator /(BigNumber a, BigNumber b)
        {
            if (b.Mantissa == 0) throw new DivideByZeroException("Cannot divide a BigNumber by zero.");
            if (a.Mantissa == 0) return Zero;
            return new BigNumber(a.Mantissa / b.Mantissa, a.Exponent - b.Exponent);
        }

        /// <summary>Adds a <see cref="BigNumber"/> and an <see cref="int"/>.</summary>
        public static BigNumber operator +(BigNumber a, int b)    => a + (BigNumber)b;
        /// <summary>Adds an <see cref="int"/> and a <see cref="BigNumber"/>.</summary>
        public static BigNumber operator +(int a,    BigNumber b) => (BigNumber)a + b;
        /// <summary>Adds a <see cref="BigNumber"/> and a <see cref="long"/>.</summary>
        public static BigNumber operator +(BigNumber a, long b)   => a + (BigNumber)b;
        /// <summary>Adds a <see cref="long"/> and a <see cref="BigNumber"/>.</summary>
        public static BigNumber operator +(long a,   BigNumber b) => (BigNumber)a + b;
        /// <summary>Adds a <see cref="BigNumber"/> and a <see cref="double"/>.</summary>
        public static BigNumber operator +(BigNumber a, double b) => a + (BigNumber)b;
        /// <summary>Adds a <see cref="double"/> and a <see cref="BigNumber"/>.</summary>
        public static BigNumber operator +(double a, BigNumber b) => (BigNumber)a + b;

        /// <summary>Subtracts an <see cref="int"/> from a <see cref="BigNumber"/>.</summary>
        public static BigNumber operator -(BigNumber a, int b)    => a - (BigNumber)b;
        /// <summary>Subtracts a <see cref="BigNumber"/> from an <see cref="int"/>.</summary>
        public static BigNumber operator -(int a,    BigNumber b) => (BigNumber)a - b;
        /// <summary>Subtracts a <see cref="long"/> from a <see cref="BigNumber"/>.</summary>
        public static BigNumber operator -(BigNumber a, long b)   => a - (BigNumber)b;
        /// <summary>Subtracts a <see cref="BigNumber"/> from a <see cref="long"/>.</summary>
        public static BigNumber operator -(long a,   BigNumber b) => (BigNumber)a - b;
        /// <summary>Subtracts a <see cref="double"/> from a <see cref="BigNumber"/>.</summary>
        public static BigNumber operator -(BigNumber a, double b) => a - (BigNumber)b;
        /// <summary>Subtracts a <see cref="BigNumber"/> from a <see cref="double"/>.</summary>
        public static BigNumber operator -(double a, BigNumber b) => (BigNumber)a - b;

        /// <summary>Multiplies a <see cref="BigNumber"/> by an <see cref="int"/>.</summary>
        public static BigNumber operator *(BigNumber a, int b)    => a * (BigNumber)b;
        /// <summary>Multiplies an <see cref="int"/> by a <see cref="BigNumber"/>.</summary>
        public static BigNumber operator *(int a,    BigNumber b) => (BigNumber)a * b;
        /// <summary>Multiplies a <see cref="BigNumber"/> by a <see cref="long"/>.</summary>
        public static BigNumber operator *(BigNumber a, long b)   => a * (BigNumber)b;
        /// <summary>Multiplies a <see cref="long"/> by a <see cref="BigNumber"/>.</summary>
        public static BigNumber operator *(long a,   BigNumber b) => (BigNumber)a * b;
        /// <summary>Multiplies a <see cref="BigNumber"/> by a <see cref="double"/>.</summary>
        public static BigNumber operator *(BigNumber a, double b) => a * (BigNumber)b;
        /// <summary>Multiplies a <see cref="double"/> by a <see cref="BigNumber"/>.</summary>
        public static BigNumber operator *(double a, BigNumber b) => (BigNumber)a * b;

        /// <summary>Divides a <see cref="BigNumber"/> by an <see cref="int"/>.</summary>
        public static BigNumber operator /(BigNumber a, int b)    => a / (BigNumber)b;
        /// <summary>Divides an <see cref="int"/> by a <see cref="BigNumber"/>.</summary>
        public static BigNumber operator /(int a,    BigNumber b) => (BigNumber)a / b;
        /// <summary>Divides a <see cref="BigNumber"/> by a <see cref="long"/>.</summary>
        public static BigNumber operator /(BigNumber a, long b)   => a / (BigNumber)b;
        /// <summary>Divides a <see cref="long"/> by a <see cref="BigNumber"/>.</summary>
        public static BigNumber operator /(long a,   BigNumber b) => (BigNumber)a / b;
        /// <summary>Divides a <see cref="BigNumber"/> by a <see cref="double"/>.</summary>
        public static BigNumber operator /(BigNumber a, double b) => a / (BigNumber)b;
        /// <summary>Divides a <see cref="double"/> by a <see cref="BigNumber"/>.</summary>
        public static BigNumber operator /(double a, BigNumber b) => (BigNumber)a / b;

        /// <summary>Returns <c>true</c> if <paramref name="a"/> equals <paramref name="b"/>.</summary>
        public static bool operator ==(BigNumber a, BigNumber b)  => a.Equals(b);
        /// <summary>Returns <c>true</c> if <paramref name="a"/> does not equal <paramref name="b"/>.</summary>
        public static bool operator !=(BigNumber a, BigNumber b)  => !a.Equals(b);
        /// <summary>Returns <c>true</c> if <paramref name="a"/> is less than <paramref name="b"/>.</summary>
        public static bool operator < (BigNumber a, BigNumber b)  => a.CompareTo(b) < 0;
        /// <summary>Returns <c>true</c> if <paramref name="a"/> is greater than <paramref name="b"/>.</summary>
        public static bool operator > (BigNumber a, BigNumber b)  => a.CompareTo(b) > 0;
        /// <summary>Returns <c>true</c> if <paramref name="a"/> is less than or equal to <paramref name="b"/>.</summary>
        public static bool operator <=(BigNumber a, BigNumber b)  => a.CompareTo(b) <= 0;
        /// <summary>Returns <c>true</c> if <paramref name="a"/> is greater than or equal to <paramref name="b"/>.</summary>
        public static bool operator >=(BigNumber a, BigNumber b)  => a.CompareTo(b) >= 0;

        /// <summary>
        /// Raises this <see cref="BigNumber"/> to the specified power.
        /// Supports fractional and negative powers.
        /// </summary>
        /// <param name="power">The exponent to raise this value to.</param>
        public BigNumber Pow(double power)
        {
            if (Mantissa == 0) return Zero;
            var log10    = (Math.Log10(Math.Abs(Mantissa)) + Exponent) * power;
            var exp      = (long)Math.Floor(log10);
            var mantissa = Math.Pow(10, log10 - exp);
            if (Mantissa < 0 && power % 2 != 0) mantissa = -mantissa;
            return new BigNumber(mantissa, exp);
        }

        /// <summary>
        /// Returns the base-10 logarithm of this value as a <see cref="double"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the value is zero or negative.</exception>
        public double Log10()
        {
            if (Mantissa <= 0) throw new InvalidOperationException("Log10 is undefined for zero or negative BigNumber.");
            return Math.Log10(Mantissa) + Exponent;
        }

        /// <summary>
        /// Returns the square root of this value.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the value is negative.</exception>
        public BigNumber Sqrt()
        {
            if (Mantissa < 0) throw new InvalidOperationException("Square root is undefined for negative BigNumber.");
            if (Mantissa == 0) return Zero;
            return Exponent % 2 == 0
                ? new BigNumber(Math.Sqrt(Mantissa),      Exponent / 2)
                : new BigNumber(Math.Sqrt(Mantissa * 10), (Exponent - 1) / 2);
        }

        /// <inheritdoc/>
        public int CompareTo(BigNumber other)
        {
            if (Mantissa > 0 && other.Mantissa < 0) return  1;
            if (Mantissa < 0 && other.Mantissa > 0) return -1;
            var negative = Mantissa < 0;
            if (Exponent != other.Exponent)
                return negative ? other.Exponent.CompareTo(Exponent) : Exponent.CompareTo(other.Exponent);
            return Mantissa.CompareTo(other.Mantissa);
        }

        /// <inheritdoc/>
        public bool Equals(BigNumber other) => Mantissa.Equals(other.Mantissa) && Exponent == other.Exponent;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is BigNumber other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Mantissa, Exponent);

        /// <summary>
        /// Returns the raw normalized representation (<c>mantissa e exponent</c>), suitable
        /// for debugging, logging, and round-tripping through <see cref="TryParse"/>.
        /// For display-ready output use <see cref="ToFormattedString()"/> or
        /// <see cref="BigNumberFormatter.Format(BigNumber)"/>.
        /// </summary>
        public override string ToString() => IsZero ? "0" : $"{Mantissa}e{Exponent}";

        /// <summary>
        /// Formats this value for display using <see cref="BigNumberFormatter.Current"/>
        /// and default <see cref="FormatOptions"/>.
        /// </summary>
        public string ToFormattedString() => BigNumberFormatter.Format(this);

        /// <summary>
        /// Formats this value for display using <see cref="BigNumberFormatter.Current"/>
        /// and the supplied <paramref name="options"/>.
        /// </summary>
        public string ToFormattedString(FormatOptions options) => BigNumberFormatter.Format(this, options);

        /// <summary>
        /// Formats this value using the notation specified by <paramref name="format"/>.
        /// </summary>
        /// <param name="format">
        /// <list type="table">
        ///   <item><term><c>"S"</c> or <c>null</c></term><description>Short suffix notation (e.g. <c>1.5K</c>)</description></item>
        ///   <item><term><c>"L"</c></term><description>Long name notation (e.g. <c>1.5 Thousand</c>)</description></item>
        ///   <item><term><c>"E"</c></term><description>Scientific notation (e.g. <c>1.5e3</c>)</description></item>
        ///   <item><term><c>"G"</c></term><description>Engineering notation (e.g. <c>1.5e3</c>, exponent always a multiple of 3)</description></item>
        /// </list>
        /// </param>
        /// <param name="formatProvider">Ignored. Pass <c>null</c>.</param>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            var notation = format?.ToUpperInvariant() switch
            {
                "L" => NumberNotation.Long,
                "E" => NumberNotation.Scientific,
                "G" => NumberNotation.Engineering,
                _   => NumberNotation.Short
            };
            return BigNumberFormatter.Format(this, new FormatOptions { notation = notation });
        }
    }
}
