using nagiashraf.CoursesApp.Services.Catalog.Core.Extensions;
using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.Core.ValidationAttributes;

public class MinWordsCountAttribute : ValidationAttribute
{
    public MinWordsCountAttribute(int minWordsCount)
        => MinWordsCount = minWordsCount;

    public int MinWordsCount { get; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        var str = value.ToString();

        var wordsCount = str?.GetWordsCount();

        if (wordsCount < MinWordsCount)
        {
            return new ValidationResult($"This field should be at least {MinWordsCount} words long.");
        }

        return ValidationResult.Success;
    }
}
