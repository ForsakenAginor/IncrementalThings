namespace ScriptGoblin.NumberForge.Formatting
{
    /// <summary>
    /// Formats a <see cref="BigNumber"/> as a human-readable string.
    /// Implement this interface to create a custom notation and assign your
    /// implementation to <see cref="BigNumberFormatter.Current"/>.
    /// </summary>
    public interface IBigNumberFormatter
    {
        /// <summary>
        /// Converts <paramref name="value"/> to a string.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="options">
        /// Display options such as decimal places and sign characters.
        /// When <c>null</c>, the formatter uses its own defaults.
        /// </param>
        /// <returns>A human-readable representation of <paramref name="value"/>.</returns>
        string Format(BigNumber value, FormatOptions options = null);
    }
}
