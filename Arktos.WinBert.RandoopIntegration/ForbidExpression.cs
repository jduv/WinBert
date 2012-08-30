namespace Arktos.WinBert.RandoopIntegration.Xml
{
    using System;
    using System.Text.RegularExpressions;
    using Common;

    /// <summary>
    /// Partial declaration of the ForbidExpression class.
    /// </summary>
    public partial class ForbidExpression
    {
        #region Public Methods

        /// Evaluates the target string to determine if this expression matches it or not.
        /// </summary>
        /// <param name="input">
        /// The input string to test.
        /// </param>
        /// <returns>
        /// True if the pattern to test evaluates to a match based on the definition 
        ///   of the expression, false otherwise.
        /// </returns>
        public bool IsForbidden(string input)
        {
            switch (this.Type)
            {
                case PatternType.ExactString:

                    // the strictest comparison
                    return this.Pattern.Equals(input);

                case PatternType.Regex:

                    // this is probably the best one to use
                    return Regex.IsMatch(input, this.Pattern);

                case PatternType.Wildcard:

                    // uses a built in class inside randoop because I'm lazy :D
                    return WildcardMatcher.Matches(this.Pattern, input);

                default:
                    throw new Exception("Unknown PatternType when evaluating the ForbidExpression!");
            }
        }
        
        #endregion
    }
}
