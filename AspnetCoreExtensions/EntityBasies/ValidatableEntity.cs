using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AspnetCoreExtensions.EntityBasies
{
    /// <summary>
    /// Abstract class for all kind of business entity classes
    /// </summary>
    public abstract class ValidatableEntity
    {
        /// <summary>
        /// Determines if current business object has no validation issue or error.
        /// </summary>
        [NotMapped]
        protected bool IsValid { get; private set; }

        /// <summary>
        /// Represents all the validation result of current business object.
        /// This property is influenced by Validate method call.
        /// </summary>
        [NotMapped]
        public IList<ValidationResult> ValidationResults { get; private set; }

        /// <summary>
        /// Validates all properties of current business object.
        /// This method influence IsValid, and ValidationResults properties.
        /// </summary>
        public virtual bool Validate()
        {
            ValidationResults = new List<ValidationResult>();

            return IsValid = Validator.TryValidateObject(this, new ValidationContext(this, null, null), ValidationResults, true);
        }
    }
}
