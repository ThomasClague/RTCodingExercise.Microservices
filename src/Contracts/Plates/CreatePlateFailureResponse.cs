namespace Contracts.Plates
{
    public record CreatePlateFailureResponse
    {
        public IReadOnlyList<string> Errors { get; init; }

        public CreatePlateFailureResponse(IEnumerable<string> errors)
        {
            Errors = errors.ToList();
        }
    }
} 