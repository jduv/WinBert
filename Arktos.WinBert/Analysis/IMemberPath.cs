namespace Arktos.WinBert.Differencing
{
    public interface IMemberPath
    {
        #region Properties

        /// <summary>
        /// Gets the path to the member, in object dot (.) notation.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the short type name of the most local member in the path (i.e. the one after the last dot)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the fully qualified type name of the most local member in the path (i.e. the one after th last dot)
        /// </summary>
        string FullName { get; }

        #endregion
    }
}
