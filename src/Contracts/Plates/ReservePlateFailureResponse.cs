namespace Contracts.Plates;

public record ReservePlateFailureResponse
{
    public IEnumerable<string> Errors { get; }

    public ReservePlateFailureResponse(IEnumerable<string> errors)
    {
        Errors = errors;
    }
} 