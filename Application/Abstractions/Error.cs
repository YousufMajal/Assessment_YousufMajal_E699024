namespace Application.Abstractions;

public class Error
{
    public Error(string code, string? description = null)
    {
        Code = code;
        Description = description ?? string.Empty;
    }

    public string Code { get; }
    public string Description { get; }

    public static Error None => new(string.Empty, string.Empty);

    public static implicit operator Error(string Description) => new(Description, Description);

    public static implicit operator string(Error error) => error.Description;
}