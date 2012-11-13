namespace Arktos.WinBert.Remoting
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Allows for isolated creation of an object of type T imported into another
    /// application domain.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object to import. Must be a deriviative of MarshalByRefObject.
    /// </typeparam>
    public sealed class Isolated<T> : IDisposable where T : MarshalByRefObject
    {
        #region Fields & Constants

        private AppDomain domain;
        private T value;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the Isolated class.
        /// </summary>
        public Isolated()
            : this(null)
        {
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the Isolated class.
        /// </summary>
        /// <param name="constructorArgs">
        /// Constructor arguments.
        /// </param>
        public Isolated(params Object[] constructorArgs)
        {
            this.domain = AppDomain.CreateDomain(
                "Isolated:" + Guid.NewGuid(),
                null,
                AppDomain.CurrentDomain.SetupInformation);

            Type type = typeof(T);
            value = (T)domain.CreateInstanceAndUnwrap(
                type.Assembly.FullName,
                type.FullName,
                false,
                BindingFlags.CreateInstance,
                null,
                constructorArgs,
                null,
                null);
        }

        /// <summary>
        /// Gets the wrapped value.
        /// </summary>
        public T Value
        {
            get
            {
                return value;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (domain != null)
            {
                AppDomain.Unload(domain);

                domain = null;
            }
        }
    }
}
