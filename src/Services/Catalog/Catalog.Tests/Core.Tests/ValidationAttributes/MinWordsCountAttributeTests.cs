using nagiashraf.CoursesApp.Services.Catalog.Core.ValidationAttributes;

namespace nagiashraf.CoursesApp.Services.Catalog.Tests.Core.Tests.ValidationAttributes;

public class MinWordsCountAttributeTests
{
    [Fact]
    public void MinWordsCountAttributeTests_ValidResult()
    {
        //Arrange
        var minWordsCount = 5;
        var attribute = new MinWordsCountAttribute(minWordsCount);
        var word = "one two three four five";

        //Act
        var actualResult = attribute.IsValid(word);

        //Assert
        Assert.True(actualResult);
    }

    [Fact]
    public void MinWordsCountAttributeTests_InvalidResult()
    {
        //Arrange
        var minWordsCount = 5;
        var attribute = new MinWordsCountAttribute(minWordsCount);
        var word = " one two    three  four ";

        //Act
        var actualResult = attribute.IsValid(word);

        //Assert
        Assert.False(actualResult);
    }
}
