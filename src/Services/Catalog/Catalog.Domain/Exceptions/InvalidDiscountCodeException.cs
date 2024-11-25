namespace Catalog.Domain.Exceptions
{
    public class InvalidDiscountCodeException : Exception
    {
        public InvalidDiscountCodeException(string discountCode)
            : base($"The discount code '{discountCode}' is not valid")
        {
        }
    }
} 