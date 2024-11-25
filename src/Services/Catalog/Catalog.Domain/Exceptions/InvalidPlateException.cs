namespace Catalog.Domain.Exceptions
{
    public class InvalidPlateException : Exception
    {
        public IReadOnlyList<string> Errors { get; }

        public InvalidPlateException(IEnumerable<string> errors) 
            : base(string.Join(", ", errors))
        {
            Errors = errors.ToList();
        }
    }
} 