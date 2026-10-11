namespace ScriptGoblin.NumberForge.Formatting
{
    /// <summary>
    /// Central formatting entry point for <see cref="BigNumber"/> values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Quick start:</b> call <see cref="Format(BigNumber)"/> for a zero-config string using
    /// the default short-suffix notation, or pass a <see cref="FormatOptions"/> instance to
    /// control notation, decimal places, sign characters, and trailing-zero trimming.
    /// </para>
    /// <para>
    /// <b>Global default:</b> replace <see cref="Current"/> once at startup to change the
    /// notation used by <see cref="BigNumber.ToString()"/> and <see cref="Format(BigNumber)"/>
    /// throughout your project:
    /// <code>
    /// BigNumberFormatter.Current = new ScientificFormat();
    /// </code>
    /// </para>
    /// <para>
    /// <b>Custom notations:</b> implement <see cref="IBigNumberFormatter"/> and assign it to
    /// <see cref="Current"/> or pass it directly via the formatter classes.
    /// </para>
    /// </remarks>
    public static class BigNumberFormatter
    {
        /// <summary>
        /// The formatter used by <see cref="Format(BigNumber)"/> and <see cref="BigNumber.ToString()"/>.
        /// Defaults to <see cref="ShortFormat"/>. Replace at startup to change the global default.
        /// </summary>
        public static IBigNumberFormatter Current = new ShortFormat();

        private static readonly ShortFormat       _short       = new();
        private static readonly LongFormat        _long        = new();
        private static readonly ScientificFormat  _scientific  = new();
        private static readonly EngineeringFormat _engineering = new();

        /// <summary>
        /// Formats <paramref name="value"/> using <see cref="Current"/> and default
        /// <see cref="FormatOptions"/>.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>A formatted string.</returns>
        public static string Format(BigNumber value) => Current.Format(value);

        /// <summary>
        /// Formats <paramref name="value"/> using the notation specified in
        /// <paramref name="options"/>. The formatter is selected automatically based on
        /// <see cref="FormatOptions.notation"/>; <see cref="Current"/> is not used.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="options">Formatting options including notation style.</param>
        /// <returns>A formatted string.</returns>
        public static string Format(BigNumber value, FormatOptions options)
        {
            IBigNumberFormatter formatter = options.notation switch
            {
                NumberNotation.Long        => _long,
                NumberNotation.Scientific  => _scientific,
                NumberNotation.Engineering => _engineering,
                _                          => _short
            };
            return formatter.Format(value, options);
        }
    }
}
