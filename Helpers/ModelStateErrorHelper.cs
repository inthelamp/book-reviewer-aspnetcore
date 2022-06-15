using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BookReviewer.Helpers
{
    public static class ModelStateErrorHelper
    {
        /// <summary>
        /// Returns a Key/Value pair with all the errors in the model
        /// according to the data annotation properties.
        /// </summary>
        /// <param name="errorDictionary"></param>
        /// <returns>
        /// Key: Name of the property
        /// Value: The list of error messages returned from data annotation
        /// </returns>
        public static Dictionary<string, List<string>> GetModelErrors(this ModelStateDictionary errorDictionary)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            foreach (KeyValuePair<string, ModelStateEntry> kvp in errorDictionary)
            {
                string key = kvp.Key;
                ModelStateEntry entry = kvp.Value;

                if (entry.Errors.Count > 0)
                {
                    List<string> errorList = new List<string>();
                    foreach (ModelError error in entry.Errors)
                    {
                        errorList.Add(error.ErrorMessage);
                    }

                    errors[key] = errorList;
                }
            }

            return errors;
        }

        /// <summary>
        /// Returns a string which contains all the error messages separated by semicolon
        /// </summary>
        /// <param name="errorDictionary"></param>
        /// <returns>
        /// A string combining all the error messages
        /// </returns>
        public static string GetModelErrorMessages(this ModelStateDictionary errorDictionary)
        {
            var errorMessages = string.Join("; ", errorDictionary.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

            return errorMessages;                         
        }
    }
}