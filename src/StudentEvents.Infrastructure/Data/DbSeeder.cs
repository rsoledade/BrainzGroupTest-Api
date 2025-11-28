using StudentEvents.Domain.Entities;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace StudentEvents.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(StudentEventsDbContext db, IConfiguration? configuration = null)
        {
            if (!await db.Users.AnyAsync())
            {
                var defaultPassword = configuration?["TestUsers:DefaultPassword"] ?? "123456";

                var admin = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@school.local",
                    DisplayName = "Admin User",
                    PasswordHash = HashPassword(defaultPassword)
                };

                var userPadrao = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "userpadrao@school.local",
                    DisplayName = "Usuario Padrao",
                    PasswordHash = HashPassword(defaultPassword)
                };

                db.Users.Add(admin);
                db.Users.Add(userPadrao);
                await db.SaveChangesAsync();
            }
        }

        // synchronous wrapper kept for compatibility
        public static void Seed(StudentEventsDbContext db)
        {
            SeedAsync(db).GetAwaiter().GetResult();
        }

        private static string HashPassword(string password)
        {
            const int SaltSize = 16; //128 bit
            const int KeySize = 32; //256 bit
            const int Iterations = 10000;

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);

            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }
    }
}