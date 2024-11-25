using Catalog.Domain.Exceptions;

namespace Catalog.Domain.ValueObjects
{
    public class Price
    {
        private const decimal MarkupPercentage = 0.2m;
        private const decimal MinimumPricePercentage = 0.9m;

        private readonly decimal _basePrice;
        private readonly string? _discountCode;

        private Price(decimal basePrice, string? discountCode = null)
        {
            _basePrice = basePrice;
            _discountCode = discountCode;
        }

        public static Price Create(decimal basePrice, string? discountCode = null)
        {
            return new Price(basePrice, discountCode);
        }

        private decimal GetMarkedUpPrice() => _basePrice * (1 + MarkupPercentage);

        public decimal GetFinalPrice()
        {
            var markedUpPrice = GetMarkedUpPrice();
            
            if (string.IsNullOrEmpty(_discountCode))
                return markedUpPrice;

            return CalculateDiscountedPrice(markedUpPrice);
        }

        public void ValidateDiscount()
        {
            if (string.IsNullOrEmpty(_discountCode))
                return;

            if (!IsValidDiscountCode(_discountCode))
            {
                throw new InvalidDiscountCodeException(_discountCode);
            }

            var markedUpPrice = GetMarkedUpPrice();
            var minimumAllowedPrice = markedUpPrice * MinimumPricePercentage;
            var discountedPrice = CalculateDiscountedPrice(markedUpPrice);

            if (discountedPrice < minimumAllowedPrice)
            {
                throw new DiscountExceedsMinimumPriceException(_discountCode, discountedPrice, minimumAllowedPrice);
            }
        }

        private decimal CalculateDiscountedPrice(decimal markedUpPrice)
        {
            return _discountCode?.ToUpper() switch
            {
                "DISCOUNT" => Math.Max(markedUpPrice - 25, 0),
                "PERCENTOFF" => markedUpPrice * 0.85m,
                _ => markedUpPrice
            };
        }

        public static bool IsValidDiscountCode(string? code)
        {
            if (string.IsNullOrEmpty(code))
                return true;

            return new[] { "DISCOUNT", "PERCENTOFF" }.Contains(code.ToUpper());
        }
    }
} 