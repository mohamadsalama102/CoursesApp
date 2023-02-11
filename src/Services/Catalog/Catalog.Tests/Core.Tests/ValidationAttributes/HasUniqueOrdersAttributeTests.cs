using nagiashraf.CoursesApp.Services.Catalog.API.DTOs;
using nagiashraf.CoursesApp.Services.Catalog.Core.ValidationAttributes;

namespace nagiashraf.CoursesApp.Services.Catalog.Tests.Core.Tests.ValidationAttributes;

public class HasUniqueOrdersAttributeTests
{
    [Fact]
    public void HasUniqueOrdersAttribute_ValidResult()
    {
        //Arrange
        var sections = new List<SectionDto> { new SectionDto { Order = 1 }, new SectionDto { Order = 3 } };
        var attribute = new HasUniqueOrdersAttribute();

        //Act
        var actualResult = attribute.IsValid(sections);

        //Assert
        Assert.True(actualResult);
    }
}
