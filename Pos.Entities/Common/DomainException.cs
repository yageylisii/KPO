namespace Pos.Entities.Common;

// Нарушение инварианта
// Бросается моделью, а не UI: «нельзя поставить 12 из 10» знает сам домен
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
