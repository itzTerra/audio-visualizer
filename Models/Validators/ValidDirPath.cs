using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace AudioVisualizer.Models.Validators;

// Inspired by: https://techfilth.blogspot.com/2011/07/taking-data-binding-validation-and-mvvm.html
public class ValidDirPath : ValidationAttribute
{
    /// <summary>
    /// Gets and sets a flag indicating whether an empty path forms an error condition or not.
    /// </summary>
    public bool AllowEmptyPath { get; set; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value != null && value.GetType() != typeof(string))
            return new ValidationResult("Input value was of the wrong type, expected a string");

        var filePath = value as string;

        //check for empty/null file path:
        if (string.IsNullOrEmpty(filePath))
        {
            if (!AllowEmptyPath)
                return new ValidationResult("The file path may not be empty.");
            else
                return ValidationResult.Success;
        }

        //null & empty has been handled above, now check for pure whitespace:
        if (string.IsNullOrWhiteSpace(filePath))
            return new ValidationResult("The file path cannot consist only of whitespace.");

        //check the path:
        if (Path.GetInvalidPathChars().Any(x => filePath.Contains(x)))
            return new ValidationResult(string.Format("The characters {0} are not permitted in a file path.", GetPrintableInvalidChars(Path.GetInvalidPathChars())));

        return ValidationResult.Success;
    }

    private string GetPrintableInvalidChars(char[] chars)
    {
        string invalidChars = string.Join("", chars.Where(x => !char.IsWhiteSpace(x)));
        return invalidChars;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a valid directory path.";
    }
}
