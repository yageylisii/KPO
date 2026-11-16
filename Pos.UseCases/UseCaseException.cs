namespace Pos.UseCases;

// Ошибка сценария (не нарушение инварианта домена): студент не найден,
// неизвестный формат экспорта и подобные
public sealed class UseCaseException : Exception
{
    public UseCaseException(string message) : base(message)
    {
    }
}
