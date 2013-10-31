namespace Arktos.WinBert.Extensions
{
    using System.Reflection;

    /// <summary>
    /// Object extension methods.
    /// </summary>
    public static class Objects
    {
        #region Fields & Constants

        public static readonly string AnonymousTypeDesignation = "AnonymousType";

        #endregion

        #region Extension Methods

        /// <summary>
        /// Determines if the target object is primitive.
        /// </summary>
        /// <param name="obj">
        /// The object to test.
        /// </param>
        /// <returns>
        /// True if the object is primitive, false otherwise.
        /// </returns>
        public static bool IsPrimitive(this object obj)
        {
            return obj.GetType().IsPrimitive || obj is decimal || obj is string;
        }

        /// <summary>
        /// Determins if the target object *might* be an anonymous type. You could break this by simply
        /// creating a class with the CompilerGenerated attribute and adding "AnonymousType" to the name.
        /// </summary>
        /// <param name="obj">
        /// The object to test.
        /// </param>
        /// <returns></returns>
        public static bool CouldBeAnonymousType(this object obj)
        {
            var type = obj.GetType();
            return type.IsCompilerGenerated() && type.FullName.Contains(AnonymousTypeDesignation) &&
                type.Name.StartsWith("<>") && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        #endregion
    }
}
