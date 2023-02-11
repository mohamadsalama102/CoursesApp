using nagiashraf.CoursesApp.Services.Catalog.Core.Extensions;

namespace nagiashraf.CoursesApp.Services.Catalog.Tests.Core.Tests.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void CanRemoveExtraWhiteSpaces()
    {
        //Arrange
        var str = "  test   test  ";
        var ExpectedStrLength = 9;

        //Act
        var result = str.RemoveExtraWhiteSpaces();

        //Assert
        Assert.Equal(ExpectedStrLength, result.Length);
    }

    [Fact]
    public void CanGetWordsCount()
    {
        //Arrange
        var str = " one   two three four five s ";
        var ExpectedWordsCount = 6;

        //Act
        var result = str.GetWordsCount();

        //Assert
        Assert.Equal(ExpectedWordsCount, result);
    }

    [Fact]
    public void CanConvertToIEnumerableBySeparator()
    {
        //Arrange
        var str = "one--testSeparator--two";
        var resultElementsCount = 2;
        var separator = "--testSeparator--";

        //Act
        var result = str.ToListBySeparator(separator);

        //Assert
        Assert.IsType<List<string>>(result);
        Assert.Equal(resultElementsCount, result.Count);
    }
}
