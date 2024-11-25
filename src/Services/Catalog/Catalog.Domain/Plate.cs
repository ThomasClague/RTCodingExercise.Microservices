using Catalog.Domain.Exceptions;
using Catalog.Domain.Primitives;
using Contracts.Plates.Events;
using DomainEvents.Plates;
using System.ComponentModel.DataAnnotations.Schema;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain
{
    public class Plate : EntityBase
    {
        public Guid Id { get; private set; }
        public string Registration { get; private set; }
        public decimal PurchasePrice { get; private set; }
        public decimal SalePrice { get; private set; }
        public string Letters { get; private set; }
        public int Numbers { get; private set; }
        public PlateStatus Status { get; private set; }
        public int StatusId { get; private set; }

        private string? _appliedDiscountCode;

        [NotMapped]
        public decimal DisplayPrice => Price.Create(SalePrice, _appliedDiscountCode).GetFinalPrice();

        public Plate() { } // For EF Core

        public static Plate Create(string registration, decimal purchasePrice, decimal salePrice)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(registration))
            {
                errors.Add("Registration cannot be empty");
            }

            if (purchasePrice <= 0)
            {
                errors.Add("Purchase price must be greater than zero");
            }

            if (salePrice <= 0)
            {
                errors.Add("Sale price must be greater than zero");
            }

            if (salePrice <= purchasePrice)
            {
                errors.Add("Sale price must be greater than purchase price");
            }

            if (!TryParseRegistration(registration, out string letters, out int numbers))
            {
                errors.Add("Invalid registration format. Expected format: ABC123");
            }

            if (errors.Any())
            {
                throw new InvalidPlateException(errors);
            }

            var plate = new Plate
            {
                Id = Guid.NewGuid(),
                Registration = FormatRegistration(registration),
                PurchasePrice = purchasePrice,
                SalePrice = salePrice,
                Letters = letters.ToUpper(),
                Numbers = numbers,
                StatusId = (int)PlateStatusEnum.Available
            };

            plate.RegisterDomainEvent(new PlateCreatedEvent
            {
                Id = plate.Id,
                Registration = plate.Registration,
                PurchasePrice = plate.PurchasePrice,
                SalePrice = plate.SalePrice,
            });

            return plate;
        }

        public void Reserve()
        {
            if (StatusId == (int)PlateStatusEnum.Reserved)
            {
                throw new PlateAlreadyReservedException(Registration);
            }

            if (StatusId == (int)PlateStatusEnum.Sold)
            {
                throw new PlateAlreadySoldException(Registration);
            }

            StatusId = (int)PlateStatusEnum.Reserved;

            RegisterDomainEvent(new PlateReservedEvent
            {
                Id = Id,
                Registration = Registration,
            });
        }

        public void MarkAsSold(string? discountCode = null)
        {
            if (StatusId == (int)PlateStatusEnum.Sold)
            {
                throw new PlateAlreadySoldException(Registration);
            }

            if (StatusId == (int)PlateStatusEnum.Reserved)
            {
                throw new PlateAlreadyReservedException(Registration);
            }

            if (discountCode != null)
            {
                ValidateDiscount(discountCode);
                ApplyDiscount(discountCode);
            }

            StatusId = (int)PlateStatusEnum.Sold;

            RegisterDomainEvent(new PlateSoldEvent
            {
                Id = Id,
                Registration = Registration,
                SalePrice = DisplayPrice
            });
        }

        public void ApplyDiscount(string? discountCode)
        {
            _appliedDiscountCode = discountCode;
        }

        public void ValidateDiscount(string? discountCode)
        {
            var price = Price.Create(SalePrice, discountCode);
            price.ValidateDiscount();
        }

        private static bool TryParseRegistration(string registration, out string letters, out int numbers)
        {
            letters = null;
            numbers = 0;

            if (string.IsNullOrWhiteSpace(registration))
                return false;

            registration = registration.Replace(" ", "").ToUpper();

            if (!registration.All(char.IsLetterOrDigit))
                return false;

            if (!registration.Any(char.IsLetter) || !registration.Any(char.IsDigit))
                return false;

            letters = new string(registration.Where(char.IsLetter).ToArray());
            string numberPart = new string(registration.Where(char.IsDigit).ToArray());

            return int.TryParse(numberPart, out numbers);
        }

        private static string FormatRegistration(string registration)
            => registration.ToUpper().Replace(" ", "");
    }

    public enum PlateStatusEnum
    {
        Available = 1,
        Reserved = 2,
        Sold = 3
    }
}