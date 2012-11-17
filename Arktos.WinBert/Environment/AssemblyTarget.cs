namespace Arktos.WinBert.Environment
{
    /// <summary>
    /// Simple class representing an assembly target.
    /// </summary>
    public class AssemblyTarget : IAssemblyTarget
    {
        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instnce of the AssemblyTarget class.
        /// </summary>
        /// <param name="target">
        /// The path to the assembly.
        /// </param>
        public AssemblyTarget(string target)
        {
            this.Location = target;            
        }        

        #endregion

        #region Properties

        /// <inheritdoc />
        public string Location { get; private set; }

        #endregion
    }
}
