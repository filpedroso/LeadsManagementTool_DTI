namespace LeadsManagement.Application.Common.Models;

/// <summary>
/// Exceção customizada para erros da aplicação
/// </summary>
public class ApiException : Exception
{
    public int StatusCode { get; set; }
    public List<string> Errors { get; set; } = new();

    public ApiException(string message, int statusCode = 500) : base(message)
    {
        StatusCode = statusCode;
        Errors.Add(message);
    }

    public ApiException(List<string> errors, int statusCode = 500) : base(string.Join(", ", errors))
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}
