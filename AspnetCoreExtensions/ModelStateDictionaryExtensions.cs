using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace AspnetCoreExtensions
{
    public static class ModelStateDictionaryExtensions
    {
        /// <summary>
        /// Determine if current modelState is valid for specific model
        /// </summary>
        /// <param name="modelState">Current ModelState instance</param>
        /// <param name="parameterName">parameter name of the model in action method.</param>
        /// <returns></returns>
        public static bool IsValid(this ModelStateDictionary modelState, string parameterName)
        {
            return !modelState.FindKeysWithPrefix(parameterName).Any(x => x.Value.ValidationState == ModelValidationState.Invalid);
        }

        /// <summary>
        /// Retrieve All Error Message within ModelState as ValidationResult
        /// </summary>
        /// <param name="modelState">Current ModelState instance</param>
        /// <param name="parameterName">parameter name of the model in action method.</param>
        /// <returns>IEnumerable of ValidationResult</returns>
        public static IEnumerable<ValidationResult> ToValidationResult(this ModelStateDictionary modelState, string parameterName = null)
        {
            IEnumerable<KeyValuePair<string, ModelStateEntry>> collection = string.IsNullOrWhiteSpace(parameterName)
                ? modelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid)
                : modelState.FindKeysWithPrefix(parameterName);

            foreach (var item in collection)
                foreach (var error in item.Value.Errors)
                    yield return new ValidationResult(error.ErrorMessage ?? error.Exception.Message, new string[] { item.Key });
        }

        /// <summary>
        /// Retrieve all ErrorMessages concatenated in one string.
        /// </summary>
        /// <param name="modelState">Current ModelState instance</param>
        /// <param name="parameterName">parameter name of the model in action method.</param>
        /// <returns>Concatenated error message.</returns>
        public static string ToStringAllErrors(this ModelStateDictionary modelState, string parameterName = null)
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(parameterName))
                foreach (var item in modelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid))
                    foreach (var item2 in item.Value.Errors)
                        sb.AppendLine(item2.ErrorMessage ?? item2.Exception.Message);
            else
                foreach (var item in modelState.FindKeysWithPrefix(parameterName))
                    foreach (var item2 in item.Value.Errors)
                        sb.AppendLine(item2.ErrorMessage ?? item2.Exception.Message);

            return sb.ToString();
        }

        /// <summary>
        /// Retrieve first ErrorMessage from <see cref="ModelStateDictionary"/> instance
        /// </summary>
        /// <param name="modelState">Current ModelState instance</param>
        /// <returns>ErrorMessage of first ModelState.</returns>
        public static string GetFirstError(this ModelStateDictionary modelState) =>
            modelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).FirstOrDefault();

    }
}
