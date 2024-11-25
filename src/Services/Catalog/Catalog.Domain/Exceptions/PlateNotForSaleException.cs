namespace Catalog.Domain.Exceptions
{
    public class PlateNotForSaleException : PlateExceptionBase
    {
        public PlateNotForSaleException(string registration) 
            : base($"Plate {registration} is not for sale", registration)
        {
        }
    }
} 