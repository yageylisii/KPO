using Pos.Entities.Students;
using Pos.UseCases.Ports;

namespace Pos.Infrastructure.Persistence;

// Адаптер порта IStudentRepository: хранение в памяти
public sealed class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _students = new();

    public StudentId NextId() =>
        new(_students.Select(s => s.Id.Value).DefaultIfEmpty(0).Max() + 1);

    public void Add(Student student) => _students.Add(student);

    public Student? Find(StudentId id) => _students.FirstOrDefault(s => s.Id == id);

    public IReadOnlyList<Student> GetAll() => _students.ToList();
}
