using System;

namespace ScriptGoblin.NumberForge.Formatting
{
    /// <summary>
    /// Formats a <see cref="BigNumber"/> in engineering notation: the exponent is always a
    /// multiple of three, so the mantissa is in the range [1, 1000).
    /// </summary>
    /// <remarks>
    /// This mirrors the SI prefix convention (kilo = 10³, mega = 10⁶, …) and makes values
    /// easier to mentally map to unit prefixes.
    /// <para>Examples with default <see cref="FormatOptions"/>:</para>
    /// <list type="table">
    ///   <item><term><c>1,500</c></term><description><c>1.5e3</c></description></item>
    ///   <item><term><c>15,000</c></term><description><c>15e3</c></description></item>
    ///   <item><term><c>150,000</c></term><description><c>150e3</c></description></item>
    ///   <item><term><c>1,500,000</c></term><description><c>1.5e6</c></description></item>
    /// </list>
    /// </remarks>
    public class EngineeringFormat : IBigNumberFormatter
    {
        /// <inheritdoc/>
        public string Format(BigNumber value, FormatOptions options = null)
        {
            options ??= new FormatOptions();

            if (value.Mantissa == 0) return options.FormatMantissa(0);

            var exp      = value.Exponent;
            var engExp   = (long)Math.Floor((double)exp / 3) * 3;
            var mantissa = value.Mantissa * Math.Pow(10, exp - engExp);

            return $"{options.FormatMantissa(mantissa)}e{engExp}";
        }
    }
}
