using Microsoft.EntityFrameworkCore;
using StudentEvents.Infrastructure.Data;

namespace StudentEvents.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly StudentEventsDbContext _db;
        private readonly ITokenService _tokenService;

        public AuthService(StudentEventsDbContext db, ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        public async Task<string?> AuthenticateAsync(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            var valid = PasswordHasher.Verify(password, user.PasswordHash);
            if (!valid) return null;

            return _tokenService.GenerateToken(user);
        }
    }
}
