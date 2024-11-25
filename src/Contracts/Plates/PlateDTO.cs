using Catalog.Domain;

namespace Contracts.Plates
{
    public class PlateDTO
    {
        public Guid Id { get; init; }
        public string Registration { get; init; }
        public decimal PurchasePrice { get; init; }
        public decimal DisplayPrice { get; init; }
        public string Letters { get; init; }
        public int Numbers { get; init; }
        public int StatusId { get; init; }

        public static PlateDTO FromPlate(Plate plate)
        {
            return new PlateDTO
            {
                Id = plate.Id,
                Registration = plate.Registration,
                PurchasePrice = plate.PurchasePrice,
                DisplayPrice = plate.DisplayPrice,
                Letters = plate.Letters,
                Numbers = plate.Numbers,
                StatusId = plate.StatusId,
            };
        }
    }
}