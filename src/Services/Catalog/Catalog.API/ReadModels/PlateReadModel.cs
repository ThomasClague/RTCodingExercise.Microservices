using Contracts.Plates;

namespace Catalog.API.ReadModels
{
    public class PlateReadModel
    {
        public Guid Id { get; set; }
        public string Registration { get; set; }
        public decimal PurchasePrice { get; set; }
        public int StatusId { get; set; }
        public decimal DisplayPrice { get; set; }

        public static PlateReadModel FromPlate(Plate plate, string? discountCode)
        {
            plate.ApplyDiscount(discountCode);
            
            return new PlateReadModel
            {
                Id = plate.Id,
                Registration = plate.Registration,
                PurchasePrice = plate.PurchasePrice,
                StatusId = plate.StatusId,
                DisplayPrice = plate.DisplayPrice
            };
        }

        public PlateDTO ToDTO()
        {
            return new PlateDTO
            {
                Id = Id,
                Registration = Registration,
                PurchasePrice = PurchasePrice,
                StatusId = StatusId,
                DisplayPrice = DisplayPrice
            };
        }
    }
} 