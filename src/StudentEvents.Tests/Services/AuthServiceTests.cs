using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StudentEvents.Infrastructure.Data;
using StudentEvents.Application.Services;
using System.Threading.Tasks;

namespace StudentEvents.Tests.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task AuthenticateAsync_ReturnsToken_WhenCredentialsValid()
        {
            var options = new DbContextOptionsBuilder<StudentEventsDbContext>()
            .UseInMemoryDatabase("AuthTestDb")
            .Options;

            using var db = new StudentEventsDbContext(options);
            // seed user
            var user = new StudentEvents.Domain.Entities.User { Id = System.Guid.NewGuid(), Email = "test@local", DisplayName = "Test", PasswordHash = StudentEvents.Application.Services.PasswordHasher.Hash("pass123") };
            db.Users.Add(user);
            db.SaveChanges();

            var tokenService = new TokenService(new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddInMemoryCollection().Build());
            var authService = new AuthService(db, tokenService);

            var token = await authService.AuthenticateAsync("test@local", "pass123");
            token.Should().NotBeNullOrEmpty();
        }
    }
}
