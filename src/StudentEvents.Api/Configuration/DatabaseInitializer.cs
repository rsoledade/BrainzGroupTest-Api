using Microsoft.EntityFrameworkCore;
using StudentEvents.Infrastructure.Data;

namespace StudentEvents.Api.Configuration
{
    public class DatabaseInitializer
    {
        private readonly IServiceProvider _services;
        public DatabaseInitializer(IServiceProvider services)
        {
            _services = services;
        }

        public async Task InitializeAsync()
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<StudentEventsDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseInitializer>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            try
            {
                var pending = await db.Database.GetPendingMigrationsAsync();
                if (pending != null && pending.Any())
                {
                    logger.LogInformation("Applying {Count} pending migrations...", pending.Count());
                    await db.Database.MigrateAsync();
                }
                else
                {
                    logger.LogInformation("No pending migrations detected.");
                }

                // Only seed when there are no users
                if (!await db.Users.AnyAsync())
                {
                    logger.LogInformation("No users found in database. Running seed...");
                    await DbSeeder.SeedAsync(db, configuration);
                }
                else
                {
                    logger.LogInformation("Users already present in database. Skipping seeding.");
                }
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2714)
            {
                logger.LogWarning(ex, "Migration skipped because an object already exists in the database. Ensure __EFMigrationsHistory is in sync with the schema.");
                // Only seed if no users exist
                if (!await db.Users.AnyAsync())
                {
                    logger.LogInformation("No users found in database after migration conflict. Running seed...");
                    await DbSeeder.SeedAsync(db, configuration);
                }
                else
                {
                    logger.LogInformation("Users already present in database after migration conflict. Skipping seeding.");
                }
            }
        }
    }
}
