using System.ComponentModel.DataAnnotations;
using BookReviewer.Helpers;

namespace BookReviewer.CustomValidation  
{  
    public class ContentAttribute : ValidationAttribute  
    {  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)  
        {  
            var validationResult = ValidationResult.Success;
            string content = RichEditorHelper.GetContentText(value.ToString());

            // Checks the length of the content
            if (content.Length == 0)
            {
                string errorMessage = "There is no review to save.";
                validationResult = new ValidationResult(errorMessage);
            }

            return validationResult;
        }
    }  
}