using StudentEvents.Domain.Entities;
using System.Security.Cryptography;

namespace StudentEvents.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static void Seed(StudentEventsDbContext db)
        {
            if (!db.Users.Any())
            {
                var admin = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@school.local",
                    DisplayName = "Admin User",
                    PasswordHash = HashPassword("123456")
                };

                var userPadrao = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "userpadrao@school.local",
                    DisplayName = "Usuario Padrao",
                    PasswordHash = HashPassword("123456")
                };

                db.Users.Add(admin);
                db.Users.Add(userPadrao);
                db.SaveChanges();
            }
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