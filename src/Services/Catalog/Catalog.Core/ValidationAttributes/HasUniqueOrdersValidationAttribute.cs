using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.Core.ValidationAttributes;

public class HasUniqueOrdersAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IEnumerable)
            return ValidationResult.Success;

        var genericTypeProperties = value
            .GetType()
            .GetGenericArguments()[0]
            .GetProperties();

        var orderProperty = genericTypeProperties.SingleOrDefault(p => p.Name == "Order");
        if (orderProperty == null || orderProperty.PropertyType != typeof(int))
        {
            return ValidationResult.Success;
        }

        var collectionElements = (IEnumerable)value;
        var uniqueOrders = new HashSet<int>();

        foreach (var element in collectionElements)
        {
            var orderValue = (int)element.GetType().GetProperty("Order")!.GetValue(element)!;
            if (!uniqueOrders.Add(orderValue))
                return new ValidationResult($"Orders of {validationContext.MemberName} must be unique");
        }

        return ValidationResult.Success;
    }
}
