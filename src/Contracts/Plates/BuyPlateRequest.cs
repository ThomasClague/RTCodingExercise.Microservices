namespace Contracts.Plates
{
    public class BuyPlateRequest
    {
        public Guid PlateId { get; init; }
        public string? DiscountCode { get; init; }
    }
} 