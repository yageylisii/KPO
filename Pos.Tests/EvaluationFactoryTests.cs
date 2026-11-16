using Pos.Entities.Common;
using Pos.Entities.Evaluations;
using Pos.Entities.Rubrics;
using Pos.Entities.Students;
using Xunit;

namespace Pos.Tests;

public class EvaluationFactoryTests
{
    private static readonly DateOnly Today = new(2026, 11, 9);

    private readonly EvaluationFactory _factory = new();
    private readonly Rubric _rubric = new();
    private readonly Criterion _homework;

    public EvaluationFactoryTests()
    {
        _homework = _rubric.AddCriterion("ДЗ", new MaxScore(10), new Weight(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7.5)]
    [InlineData(10)]
    public void Create_ScoreWithinCriterion_ShouldCreateEvaluation(decimal score)
    {
        // Act
        var evaluation = _factory.Create(new EvaluationId(1), _rubric, new StudentId(2), _homework.Id, score, Today);

        // Assert
        Assert.Equal(score, evaluation.Score.Value);
        Assert.Equal(_homework.Id, evaluation.CriterionId);
    }

    [Theory]
    [InlineData(12)]
    [InlineData(10.01)]
    [InlineData(-1)]
    public void Create_ScoreOutsideCriterion_ShouldThrow(decimal score)
    {
        Assert.Throws<DomainException>(() =>
            _factory.Create(new EvaluationId(1), _rubric, new StudentId(2), _homework.Id, score, Today));
    }

    [Fact]
    public void Create_CriterionNotInRubric_ShouldThrow()
    {
        Assert.Throws<DomainException>(() =>
            _factory.Create(new EvaluationId(1), _rubric, new StudentId(2), new CriterionId(999), 5, Today));
    }

    [Fact]
    public void Correct_AboveMax_ShouldThrowAndKeepOldScore()
    {
        // Arrange
        var evaluation = _factory.Create(new EvaluationId(1), _rubric, new StudentId(2), _homework.Id, 8, Today);

        // Act & Assert
        Assert.Throws<DomainException>(() => evaluation.Correct(_homework, new Score(12), Today, null));
        Assert.Equal(8, evaluation.Score.Value);
    }
}
