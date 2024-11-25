namespace Catalog.Domain.Exceptions;

public class PlateAlreadyReservedException : PlateExceptionBase
{
    public PlateAlreadyReservedException(string registration) 
        : base($"Plate {registration} is already reserved", registration)
    {
    }
} 