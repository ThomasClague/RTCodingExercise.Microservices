using Audit.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Audit.API.Data
{
    public class AuditDbContext : DbContext
    {
        public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
        {
        }

        public DbSet<AuditRecord> AuditRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditRecord>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.EventType).IsRequired();
                builder.Property(x => x.EntityType).IsRequired();
                builder.Property(x => x.EntityId).IsRequired();
                builder.Property(x => x.Data).IsRequired();
                builder.Property(x => x.Timestamp).IsRequired();
            });
        }
    }
}

