using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.API.Data.Configurations
{
    public class PlateStatusConfiguration : IEntityTypeConfiguration<PlateStatus>
    {
        public void Configure(EntityTypeBuilder<PlateStatus> builder)
        {
            builder.Property(x => x.Description)
                .IsRequired();

            builder.HasData(
                PlateStatus.FromEnum(PlateStatusEnum.Available),
                PlateStatus.FromEnum(PlateStatusEnum.Reserved),
                PlateStatus.FromEnum(PlateStatusEnum.Sold)
            );
        }
    }
} 