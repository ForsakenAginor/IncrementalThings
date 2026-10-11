namespace ScriptGoblin.NumberForge.Formatting
{
    /// <summary>
    /// Controls which notation style <see cref="BigNumberFormatter"/> uses when formatting
    /// a <see cref="BigNumber"/>.
    /// </summary>
    public enum NumberNotation
    {
        /// <summary>
        /// Short suffix notation. Values are grouped in thousands and labelled with
        /// abbreviated suffixes from <see cref="BigNumberNames.ShortNames"/>.
        /// Example: <c>1,500,000</c> → <c>1.5M</c>.
        /// </summary>
        Short,

        /// <summary>
        /// Long name notation. Values are grouped in thousands and labelled with full
        /// English names from <see cref="BigNumberNames.LongNames"/>.
        /// Example: <c>1,500,000</c> → <c>1.5 Million</c>.
        /// </summary>
        Long,

        /// <summary>
        /// Scientific notation. The mantissa is always in [1, 10) followed by the
        /// base-10 exponent.
        /// Example: <c>1,500,000</c> → <c>1.5e6</c>.
        /// </summary>
        Scientific,

        /// <summary>
        /// Engineering notation. Like scientific, but the exponent is always a multiple
        /// of three, so the mantissa is in [1, 1000).
        /// Example: <c>1,500,000</c> → <c>1.5e6</c>; <c>15,000</c> → <c>15e3</c>.
        /// </summary>
        Engineering
    }
}
