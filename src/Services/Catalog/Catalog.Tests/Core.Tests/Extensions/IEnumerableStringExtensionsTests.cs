using nagiashraf.CoursesApp.Services.Catalog.Core.Extensions;

namespace nagiashraf.CoursesApp.Services.Catalog.Tests.Core.Tests.Extensions;

public class IEnumerableStringExtensionsTests
{
    [Fact]
    public void CanConvertToStringWithSeparator()
    {
        //Arrange
        var testArray = new string[] { "one", "two" };
        var separator = "test separator";

        //Act
        var result = testArray.ToStringWithSeparator(separator);

        //Assert
        Assert.IsType<string>(result);
        Assert.Contains(separator, result);
    }
}
