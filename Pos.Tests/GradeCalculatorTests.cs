using Pos.Entities.Common;
using Pos.Entities.Evaluations;
using Pos.Entities.Grading;
using Pos.Entities.Grading.Strategies;
using Pos.Entities.Rubrics;
using Pos.Entities.Students;
using Xunit;

namespace Pos.Tests;

public class GradeCalculatorTests
{
    private static readonly DateOnly Today = new(2026, 11, 9);

    private readonly GradeCalculator _calculator = new();
    private readonly EvaluationFactory _factory = new();
    private readonly Rubric _rubric = new();
    private readonly Criterion _homework;
    private readonly Criterion _exam;
    private readonly StudentId _student = new(1);

    public GradeCalculatorTests()
    {
        _homework = _rubric.AddCriterion("ДЗ", new MaxScore(10), new Weight(0.6m));
        _exam = _rubric.AddCriterion("Экзамен", new MaxScore(5), new Weight(0.4m));
    }

    [Fact]
    public void Calculate_AllEvaluated_ShouldReturnWeightedTenPointGrade()
    {
        // Arrange: ДЗ 8/10 (0.8 × 0.6) + экзамен 5/5 (1.0 × 0.4) = 0.88 → 8.8
        var evaluations = new[]
        {
            _factory.Create(new EvaluationId(1), _rubric, _student, _homework.Id, 8, Today),
            _factory.Create(new EvaluationId(1), _rubric, _student, _exam.Id, 5, Today),
        };

        // Act
        var grade = _calculator.Calculate(_student, _rubric, evaluations, new StrictWeightedStrategy());

        // Assert
        Assert.Equal(8.8m, grade.Value);
    }

    [Fact]
    public void Calculate_MissingEvaluation_StrategiesShouldDiffer()
    {
        // Arrange: есть только ДЗ 8/10
        var evaluations = new[] { _factory.Create(new EvaluationId(1), _rubric, _student, _homework.Id, 8, Today) };

        // Act
        var strict = _calculator.Calculate(_student, _rubric, evaluations, new StrictWeightedStrategy());
        var progress = _calculator.Calculate(_student, _rubric, evaluations, new ProgressWeightedStrategy());

        // Assert
        Assert.Equal(4.8m, strict.Value);   // экзамен = 0: 0.8 × 0.6 / 1.0 × 10
        Assert.Equal(8.0m, progress.Value); // только ДЗ: 0.8 × 10
    }

    [Fact]
    public void Calculate_EvaluationOfAnotherStudent_ShouldThrow()
    {
        var foreign = _factory.Create(new EvaluationId(1), _rubric, new StudentId(2), _homework.Id, 8, Today);

        Assert.Throws<DomainException>(() =>
            _calculator.Calculate(_student, _rubric, new[] { foreign }, new StrictWeightedStrategy()));
    }

    [Fact]
    public void Calculate_EmptyRubric_ShouldThrow()
    {
        Assert.Throws<DomainException>(() =>
            _calculator.Calculate(_student, new Rubric(), Array.Empty<Evaluation>(), new StrictWeightedStrategy()));
    }
}
