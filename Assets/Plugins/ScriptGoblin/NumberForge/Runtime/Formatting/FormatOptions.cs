using System;

namespace ScriptGoblin.NumberForge.Formatting
{
    /// <summary>
    /// Controls how a <see cref="BigNumber"/> is formatted by an <see cref="IBigNumberFormatter"/>.
    /// Pass an instance to <see cref="BigNumberFormatter.Format(BigNumber, FormatOptions)"/> or
    /// directly to any formatter's <c>Format</c> method.
    /// </summary>
    public class FormatOptions
    {
        /// <summary>
        /// The maximum number of decimal places shown in the mantissa.
        /// Defaults to <c>2</c>.
        /// </summary>
        public int decimals = 2;

        /// <summary>
        /// When <c>true</c>, trailing zeros after the decimal point are omitted.
        /// For example, <c>1.50</c> becomes <c>1.5</c> and <c>1.00</c> becomes <c>1</c>.
        /// Defaults to <c>true</c>.
        /// </summary>
        public bool trimTrailingZeros = true;

        /// <summary>
        /// String prepended to positive values. Defaults to <c>""</c> (no prefix).
        /// Set to <c>"+"</c> to explicitly mark positive numbers.
        /// </summary>
        public string positiveSign = "";

        /// <summary>
        /// String prepended to negative values. Defaults to <c>"-"</c>.
        /// </summary>
        public string negativeSign = "-";

        /// <summary>
        /// The notation style to use. Only meaningful when calling
        /// <see cref="BigNumberFormatter.Format(BigNumber, FormatOptions)"/>;
        /// individual formatter classes use their own fixed notation and ignore this field.
        /// Defaults to <see cref="NumberNotation.Short"/>.
        /// </summary>
        public NumberNotation notation = NumberNotation.Short;

        /// <summary>
        /// Formats the absolute value of <paramref name="mantissa"/> using the current
        /// <see cref="decimals"/> and <see cref="trimTrailingZeros"/> settings,
        /// then prepends the appropriate sign string.
        /// </summary>
        /// <param name="mantissa">The value to format.</param>
        /// <returns>The formatted string with sign prefix applied.</returns>
        internal string FormatMantissa(double mantissa)
        {
            var fmt = trimTrailingZeros
                ? "0." + new string('#', Math.Max(0, decimals))
                : "0." + new string('0', Math.Max(0, decimals));

            var abs = Math.Abs(mantissa);
            var str = abs.ToString(fmt);
            return mantissa < 0 ? negativeSign + str : positiveSign + str;
        }
    }
}
