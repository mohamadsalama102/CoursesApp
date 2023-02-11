using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.Core.ValidationAttributes;

public class MaxLengthForIEnumerableStringElementAttribute : ValidationAttribute
{
    public MaxLengthForIEnumerableStringElementAttribute(int maxLength)
        => MaxLength = maxLength;

    public int MaxLength { get; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        var stringsEnumerable = (IEnumerable<string>)value;

        var invalidElements = new List<string>();

        foreach (var str in stringsEnumerable)
        {
            if (str.Length > MaxLength)
            {
                invalidElements.Add(str);
            }
        }

        if (invalidElements.Any())
        {
            return new ValidationResult($"The following elements exceed the max length of {MaxLength}: {string.Join(", ", invalidElements)}");
        }

        return ValidationResult.Success;
    }
}
