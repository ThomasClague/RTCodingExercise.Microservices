namespace Catalog.Domain.Exceptions
{
    public class PlateAlreadySoldException : PlateExceptionBase
    {
        public PlateAlreadySoldException(string registration) 
            : base($"Plate {registration} has already been sold", registration)
        {
        }
    }
} 