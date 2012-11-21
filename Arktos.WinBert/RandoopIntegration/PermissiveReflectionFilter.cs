namespace Arktos.WinBert.RandoopIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Arktos.WinBert.Xml;
    using Randoop;

    /// <summary>
    ///   A custom implementation of the IReflectionFilter to support using an in-memory
    ///   representation of Randoop's configuration file related to forbidden/permitted fields
    ///   for exploration. This class takes a permissive approach to avoid collisions between
    ///   inclusion/exclusion logic.
    /// </summary>
    public class PermissiveReflectionFilter : IReflectionFilter
    {
        #region Fields and Constants

        /// <summary>
        ///   A list of expressions defining forbidden fields.
        /// </summary>
        private IList<ForbidExpression> forbiddenFields  = new List<ForbidExpression>();

        /// <summary>
        ///   A list of expressions defining forbidden members.
        /// </summary>
        private IList<ForbidExpression> forbiddenMembers = new List<ForbidExpression>();

        /// <summary>
        ///   A list of expressions defining forbidden types.
        /// </summary>
        private IList<ForbidExpression> forbiddenTypes = new List<ForbidExpression>();
        
        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the list of forbidden fields for the filter.
        /// </summary>
        public IList<ForbidExpression> ForbiddenFields
        {
            get
            {
                return this.forbiddenFields;
            }

            set
            {
                this.forbiddenFields = value;
            }

        }

        /// <summary>
        ///   Gets or sets the list of forbidden members for the filter.
        /// </summary>
        public IList<ForbidExpression> ForbiddenMembers
        {
            get
            {
                return this.forbiddenMembers;
            }

            set
            {
                this.forbiddenMembers = value;
            }

        }

        /// <summary>
        ///   Gets or sets the list of forbidden types for the filter.
        /// </summary>
        public IList<ForbidExpression> ForbiddenTypes
        {
            get
            {
                return this.forbiddenTypes;
            }

            set
            {
                this.forbiddenTypes = value;
            }

        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if the target type is ok to use.
        /// </summary>
        /// <param name="type">
        /// The type to test.
        /// </param>
        /// <param name="message">
        /// A message for the caller.
        /// </param>
        /// <returns>
        /// True if the type is ok to use, false otherwise.
        /// </returns>
        public bool OkToUse(Type type, out string message)
        {
            if (type != null)
            {
                foreach (ForbidExpression expression in this.forbiddenTypes)
                {
                    if (expression.IsForbidden(type.ToString()))
                    {
                        message = string.Format(
                            "The Type {0} matches the ForbidExpression with pattern {1}. It will not be used.", 
                            type.FullName, 
                            expression.Pattern);

                        return false;
                    }
                }

                message = string.Format("The Type {0} will be used.", type.FullName);
                return true;
            }
            else
            {
                message = "The passed in Type argument was null";
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// Determines of the target constructor is ok to use.
        /// </summary>
        /// <param name="constructor">
        /// The constructor to test
        /// </param>
        /// <param name="message">
        /// A message for the caller.
        /// </param>
        /// <returns>
        /// True if the constructor is ok to use, false otherwise.
        /// </returns>
        public bool OkToUse(ConstructorInfo constructor, out string message)
        {
            if (constructor != null)
            {
                foreach (ForbidExpression fExp in this.forbiddenMembers)
                {
                    if (fExp.IsForbidden(constructor.ToString()))
                    {
                        message = string.Format("The Constructor {0} matches the ForbidExpression with pattern {1}. It will not be used.", constructor.ToString(), fExp.Pattern);
                        return false;
                    }
                }

                message = string.Format("The Constructor {0} will be used.", constructor.ToString());
                return true;
            }
            else
            {
                message = "The passed in Constructor argument was null";
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// Determines if the target method is ok to use.
        /// </summary>
        /// <param name="method">
        /// The method to test.
        /// </param>
        /// <param name="message">
        /// A message for the caller.
        /// </param>
        /// <returns>
        /// True if the method is ok to use, false otherwise.
        /// </returns>
        public bool OkToUse(MethodInfo method, out string message)
        {
            if (method != null)
            {
                foreach (ForbidExpression fExp in this.forbiddenMembers)
                {
                    if (fExp.IsForbidden(method.ToString()))
                    {
                        message = string.Format("The Method {0} matches the ForbidExpression with pattern {1}. It will not be used.", method.ToString(), fExp.Pattern);
                        return false;
                    }
                }

                message = string.Format("The Method {0} will be used.", method.ToString());
                return true;
            }
            else
            {
                message = "The passed in Method argument was null";
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// Determines if the target field is ok to use.
        /// </summary>
        /// <param name="field">
        /// The field to test.
        /// </param>
        /// <param name="message">
        /// A message for the caller.
        /// </param>
        /// <returns>
        /// True if the field is ok to use, false otherwise.
        /// </returns>
        public bool OkToUse(FieldInfo field, out string message)
        {
            if (field != null)
            {
                foreach (ForbidExpression fExp in this.forbiddenFields)
                {
                    if (fExp.IsForbidden(field.ToString()))
                    {
                        message = string.Format("The Field {0} matches the ForbidExpression with pattern {1}. It will not be used.", field.ToString(), fExp.Pattern);
                        return false;
                    }
                }

                message = string.Format("The Field {0} will be used.", field.ToString());
                return true;
            }
            else
            {
                message = "The passed in type argument was null";
                throw new ArgumentNullException(message);
            }
        }
        
        #endregion
    }
}
