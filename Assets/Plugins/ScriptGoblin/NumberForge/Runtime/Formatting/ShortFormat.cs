using System;

namespace ScriptGoblin.NumberForge.Formatting
{
    /// <summary>
    /// Formats a <see cref="BigNumber"/> using short suffix notation, grouping values by
    /// thousands and appending an abbreviated label from <see cref="BigNumberNames.ShortNames"/>.
    /// </summary>
    /// <remarks>
    /// Examples with default <see cref="FormatOptions"/>:
    /// <list type="table">
    ///   <item><term><c>999</c></term><description><c>999</c></description></item>
    ///   <item><term><c>1,500</c></term><description><c>1.5K</c></description></item>
    ///   <item><term><c>2,300,000</c></term><description><c>2.3M</c></description></item>
    ///   <item><term><c>1e100</c></term><description><c>1Ce</c> (or scientific fallback beyond the name list)</description></item>
    /// </list>
    /// Values below 1,000 are displayed as plain numbers. Values beyond the last entry in
    /// <see cref="BigNumberNames.ShortNames"/> fall back to <c>eN</c> scientific notation.
    /// To use different suffixes, assign a custom <see cref="BigNumberNames"/> subclass to
    /// <see cref="BigNumberNames.Current"/>.
    /// </remarks>
    public class ShortFormat : IBigNumberFormatter
    {
        /// <inheritdoc/>
        public string Format(BigNumber value, FormatOptions options = null)
        {
            options ??= new FormatOptions();

            if (value.Mantissa == 0) return options.FormatMantissa(0);

            if (value.Exponent < 0)
                return $"{options.FormatMantissa(value.Mantissa)}e{value.Exponent}";

            var index     = (int)(value.Exponent / 3) - 1;
            var remainder = (int)(value.Exponent % 3);
            var mantissa  = value.Mantissa * Math.Pow(10, remainder);

            if (index < 0)
                return options.FormatMantissa(mantissa);

            var names  = BigNumberNames.Current.ShortNames;
            var suffix = index < names.Length ? names[index] : $"e{value.Exponent}";
            return options.FormatMantissa(mantissa) + suffix;
        }
    }
}
