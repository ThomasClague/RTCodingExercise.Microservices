using Catalog.API.Contracts.Data;
using Catalog.Domain.Primitives;

namespace Catalog.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public IDomainEventDispatcher _dispatcher { get; init; }
        // parameterless constructor for testing
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventDispatcher dispatcher) : base(options)
        {
            _dispatcher = dispatcher;
        }

        public virtual DbSet<Plate> Plates { get; set; }
        public virtual DbSet<PlateStatus> PlateStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDbFunction(typeof(CustomDbFunctions)
                .GetMethod(nameof(CustomDbFunctions.Levenshtein)))
                .HasSchema("dbo");

            modelBuilder.HasDbFunction(typeof(CustomDbFunctions)
                .GetMethod(nameof(CustomDbFunctions.NormalizePlateNumber)))
                .HasSchema("dbo");

            // Apply all configurations in the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            if (_dispatcher == null) return result;

            if (result == 0) return result;

            var entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
            .ToArray();

            await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

            return result;
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}
