namespace Catalog.Domain.Exceptions
{
    public class DiscountExceedsMinimumPriceException : Exception
    {
        public DiscountExceedsMinimumPriceException(string discountCode, decimal discountedPrice, decimal minimumPrice)
            : base($"The discount code '{discountCode}' cannot be applied as it would reduce the price below the minimum allowed: £{minimumPrice.ToString("0.00")}")
        {
        }
    }
} 