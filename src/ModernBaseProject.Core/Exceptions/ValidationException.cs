using ModernBaseProject.Core.Constants;

namespace ModernBaseProject.Core.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors) : base(ExceptionMessages.ValidationFailed)
    {
        Errors = errors;
    }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]> { { "error", new[] { message } } };
    }
}
