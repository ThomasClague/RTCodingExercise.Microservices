namespace Catalog.Domain
{
    public class PlateStatus
    {
        public int Id { get; private set; }
        public string Description { get; private set; }

        protected PlateStatus() { } // For EF Core

        private PlateStatus(int id, string description)
        {
            Id = id;
            Description = description;
        }

        public static PlateStatus FromEnum(PlateStatusEnum status) => status switch
        {
            PlateStatusEnum.Available => new PlateStatus(
                (int)PlateStatusEnum.Available, 
                "Plate is available for purchase"),
            PlateStatusEnum.Reserved => new PlateStatus(
                (int)PlateStatusEnum.Reserved, 
                "Plate is temporarily reserved"),
            PlateStatusEnum.Sold => new PlateStatus(
                (int)PlateStatusEnum.Sold, 
                "Plate has been sold"),
            _ => throw new ArgumentException($"Unknown status: {status}")
        };
    }
} 