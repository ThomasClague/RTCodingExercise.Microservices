namespace Contracts.Plates
{
    public class BuyPlateFailureResponse
    {
        public IEnumerable<string> Errors { get; }

        public BuyPlateFailureResponse(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
} 