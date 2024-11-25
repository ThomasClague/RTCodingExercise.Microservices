using Identity.API.Contracts;
using Microsoft.Extensions.Options;

namespace Identity.API.Data
{


    public class IdentityDbContextSeed
    {
        private readonly IRoleAndUserSeeder _roleAndUserSeeder;

        public IdentityDbContextSeed(IRoleAndUserSeeder roleAndUserSeeder)
        {
            _roleAndUserSeeder = roleAndUserSeeder;
        }

        public async Task SeedAsync(
            IdentityDbContext context,
            IWebHostEnvironment env,
            ILogger<IdentityDbContextSeed> logger,
            IOptions<AppSettings> settings,
            int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            try
            {
                await _roleAndUserSeeder.SeedRolesAndUsersAsync();
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    logger.LogError(ex, "There is an error migrating data for ApplicationDbContext");
                    await SeedAsync(context, env, logger, settings, retryForAvailability);
                }
            }
        }
    }
}
