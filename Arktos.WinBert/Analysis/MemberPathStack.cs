namespace Arktos.WinBert.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Very simple override of the stack class to add a property for grabbing the current path. An instance
    /// of this class should represent the path to a member in a nested object graph. We use a stack here 
    /// instead of a simple flat list only because it's more natural during traversal of the object graph 
    /// to push and pop values off the stack as we process nodes. So this class is, mainly, just for sanity of API.
    /// </summary>
    public sealed class MemberPathStack : Stack<string>
    {
        #region Constructors & Destructors

        public MemberPathStack(string rootType)
        {
            if (string.IsNullOrEmpty(rootType))
            {
                throw new ArgumentException("Root type cannot be null or empty!");
            }
            this.Push(rootType);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current path by traversing the stack in reverse order and joining all
        /// elements using standard dot (.) notation. 
        /// </summary>
        public string CurrentPath
        {
            get
            {
                // Process in reverse order.
                return string.Join(".", this.Reverse().ToArray()).ToLower();
            }
        }

        #endregion
    }
}
