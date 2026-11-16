using Pos.Entities.Common;
using Pos.Entities.Rubrics;
using Xunit;

namespace Pos.Tests;

public class RubricTests
{
    [Fact]
    public void AddCriterion_DuplicateName_ShouldThrow()
    {
        // Arrange
        var rubric = new Rubric();
        rubric.AddCriterion("Экзамен", new MaxScore(10), new Weight(1));

        // Act & Assert
        Assert.Throws<DomainException>(() => rubric.AddCriterion("экзамен", new MaxScore(5), new Weight(1)));
    }

    [Fact]
    public void AddCriterion_ShouldNumberCriteriaSequentially()
    {
        // Arrange
        var rubric = new Rubric();

        // Act
        var first = rubric.AddCriterion("ДЗ", new MaxScore(10), new Weight(1));
        var second = rubric.AddCriterion("Экзамен", new MaxScore(10), new Weight(1));

        // Assert
        Assert.Equal(1, first.Id.Value);
        Assert.Equal(2, second.Id.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void MaxScore_NotPositive_ShouldThrow(decimal value)
    {
        Assert.Throws<DomainException>(() => new MaxScore(value));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.5)]
    public void Weight_NotPositive_ShouldThrow(decimal value)
    {
        Assert.Throws<DomainException>(() => new Weight(value));
    }
}
