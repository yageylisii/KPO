namespace Pos.UseCases.Dto;

// DTO данные для UI и адаптеров

public sealed record StudentDto(int Id, string FullName);

public sealed record CriterionDto(int Id, string Name, decimal MaxScore, decimal Weight);

public sealed record EvaluationDto(
    int Id,
    int StudentId,
    string StudentName,
    int CriterionId,
    string CriterionName,
    decimal Score,
    decimal MaxScore,
    DateOnly Date,
    string? Comment);

public sealed record GradingPolicyDto(string Name, string Description);

public sealed record CriterionResultDto(string CriterionName, decimal? Score, decimal MaxScore, decimal Weight);

public sealed record FinalGradeDto(
    int StudentId,
    string StudentName,
    decimal Value,
    string PolicyName,
    IReadOnlyList<CriterionResultDto> Breakdown);
