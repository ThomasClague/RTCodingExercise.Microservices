namespace Catalog.Domain.Exceptions
{
    public abstract class PlateExceptionBase : Exception
    {
        public string Registration { get; }

        protected PlateExceptionBase(string message, string registration) 
            : base(message)
        {
            Registration = registration;
        }
    }
} 