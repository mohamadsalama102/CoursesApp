using nagiashraf.CoursesApp.Services.Catalog.Core.ValidationAttributes;

namespace nagiashraf.CoursesApp.Services.Catalog.Tests.Core.Tests.ValidationAttributes;

public class MaxLengthForIEnumerableStringElementTests
{
    [Fact]
    public void MaxLengthForIEnumerableStringElementAttribute_InvalidResult()
    {
        //Arrange
        var maxLength = 5;
        var attribute = new MaxLengthForIEnumerableStringElementAttribute(maxLength);
        var stringArray = new string[] { "12", "12345", "123456" };

        //Act
        var result = attribute.IsValid(stringArray);

        //Assert
        Assert.False(result);
    }
}
