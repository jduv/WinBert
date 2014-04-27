namespace Arktos.WinBert.Differencing
{
    using System;

    /// <summary>
    /// Simple implementation of a member path. Nothing fancy here.
    /// </summary>
    public class MemberPath : IMemberPath
    {
        #region Fields & Constants

        public static readonly IMemberPath Empty = new MemberPath();

        #endregion

        #region Constructors & Destructors

        private MemberPath()
        {
            this.Path = string.Empty;
            this.Name = string.Empty;
            this.FullName = string.Empty;
        }

        public MemberPath(string path, string name, string fullName)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty!");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Type name cannot be null or empty!");
            }

            if (string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("Type full name cannot be null or empty!");
            }

            this.Path = path;
            this.Name = name;
            this.FullName = name;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public string Path { get; private set; }

        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public string FullName { get; private set; }

        #endregion
    }
}
