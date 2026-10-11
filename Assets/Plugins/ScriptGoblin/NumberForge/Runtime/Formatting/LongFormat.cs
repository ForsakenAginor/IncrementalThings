using System;

namespace ScriptGoblin.NumberForge.Formatting
{
    /// <summary>
    /// Formats a <see cref="BigNumber"/> using long name notation, grouping values by
    /// thousands and appending a full English name from <see cref="BigNumberNames.LongNames"/>.
    /// </summary>
    /// <remarks>
    /// Examples with default <see cref="FormatOptions"/>:
    /// <list type="table">
    ///   <item><term><c>999</c></term><description><c>999</c></description></item>
    ///   <item><term><c>1,500</c></term><description><c>1.5 Thousand</c></description></item>
    ///   <item><term><c>2,300,000</c></term><description><c>2.3 Million</c></description></item>
    /// </list>
    /// Values below 1,000 are displayed as plain numbers. Values beyond the last entry in
    /// <see cref="BigNumberNames.LongNames"/> fall back to <c>10^N</c> notation.
    /// To use custom names, assign a <see cref="BigNumberNames"/> subclass to
    /// <see cref="BigNumberNames.Current"/>.
    /// </remarks>
    public class LongFormat : IBigNumberFormatter
    {
        /// <inheritdoc/>
        public string Format(BigNumber value, FormatOptions options = null)
        {
            options ??= new FormatOptions();

            if (value.Mantissa == 0) return options.FormatMantissa(0);

            if (value.Exponent < 0)
                return $"{options.FormatMantissa(value.Mantissa)}e{value.Exponent}";

            var index    = (int)(value.Exponent / 3) - 1;
            var remainder = (int)(value.Exponent % 3);
            var mantissa  = value.Mantissa * Math.Pow(10, remainder);

            if (index < 0)
                return options.FormatMantissa(mantissa);

            var names = BigNumberNames.Current.LongNames;
            var name  = index < names.Length ? names[index] : $"10^{value.Exponent}";
            return $"{options.FormatMantissa(mantissa)} {name}";
        }
    }
}
