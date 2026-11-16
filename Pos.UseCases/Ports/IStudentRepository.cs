using Pos.Entities.Students;

namespace Pos.UseCases.Ports;

// Порт (Repository): хранение студентов снаружи домена
public interface IStudentRepository
{
    // Номер для нового объекта: максимальный из сохранённых + 1
    StudentId NextId();

    void Add(Student student);

    Student? Find(StudentId id);

    IReadOnlyList<Student> GetAll();
}
