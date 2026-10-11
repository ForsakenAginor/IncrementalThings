namespace ScriptGoblin.NumberForge.Formatting
{
    /// <summary>
    /// Formats a <see cref="BigNumber"/> in standard scientific notation:
    /// the mantissa is always in [1, 10) followed by <c>eN</c>.
    /// </summary>
    /// <remarks>
    /// Examples with default <see cref="FormatOptions"/>:
    /// <list type="table">
    ///   <item><term><c>1,500</c></term><description><c>1.5e3</c></description></item>
    ///   <item><term><c>-42,000,000</c></term><description><c>-4.2e7</c></description></item>
    ///   <item><term><c>1e100</c></term><description><c>1e100</c></description></item>
    /// </list>
    /// </remarks>
    public class ScientificFormat : IBigNumberFormatter
    {
        /// <inheritdoc/>
        public string Format(BigNumber value, FormatOptions options = null)
        {
            options ??= new FormatOptions();
            if (value.Mantissa == 0) return options.FormatMantissa(0);
            return $"{options.FormatMantissa(value.Mantissa)}e{value.Exponent}";
        }
    }
}
