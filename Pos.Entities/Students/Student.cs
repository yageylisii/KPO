using Pos.Entities.Common;

namespace Pos.Entities.Students;

// Entity «студент» — кого оценивают. Отдельный агрегат:
// студент существует независимо от рубрикатора и оценок
public sealed class Student
{
    public Student(StudentId id, string fullName)
    {
        Id = id;
        FullName = Guard.NotBlank(fullName, "ФИО студента");
    }

    public StudentId Id { get; }

    public string FullName { get; }

    public override string ToString() => FullName;
}
