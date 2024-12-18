﻿using System.Text.Json;

namespace Catalog.API.Data
{
    public class ApplicationDbContextSeed
    {
        public async Task SeedAsync(ApplicationDbContext context, IWebHostEnvironment env, ILogger<ApplicationDbContextSeed> logger, IOptions<AppSettings> settings, int? retry = 0)
        {
            int retryForAvaiability = retry.Value;

            try
            {
                await SeedCustomData(context, env, logger);
            }
            catch (Exception ex)
            {
                // used for initilisaton of docker containers
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;

                    logger.LogError(ex.Message, $"There is an error migrating data for ApplicationDbContext");

                    await SeedAsync(context, env, logger, settings, retryForAvaiability);
                }
            }
        }

        public async Task SeedCustomData(ApplicationDbContext context, IWebHostEnvironment env, ILogger<ApplicationDbContextSeed> logger)
        {
            try
            {
                var dataExists = await context.Plates.AnyAsync();
                if (dataExists)
                {
                    logger.LogWarning("Data has already been seeded");
                    return;
                }

                var plates = ReadApplicationRoleFromJson(env.ContentRootPath, logger);

                await context.Plates.AddRangeAsync(plates);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                throw;
            }
        }

        public List<Plate> ReadApplicationRoleFromJson(string contentRootPath, ILogger<ApplicationDbContextSeed> logger)
        {
            string filePath = Path.Combine(contentRootPath, "Setup", "plates.json");
            string json = File.ReadAllText(filePath);

            var plates = JsonSerializer.Deserialize<List<Plate>>(json) ?? new List<Plate>();

            return plates;
        }
    }
}
