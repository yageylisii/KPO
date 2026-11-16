namespace Pos.Entities.Common;

internal static class Guard
{
    public static string NotBlank(string? value, string what)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{what} не может быть пустым.");
        }

        return value.Trim();
    }
}
