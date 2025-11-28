using Xunit;
using FluentAssertions;
using StudentEvents.Application.Services;
using Microsoft.Extensions.Configuration;
using StudentEvents.Domain.Entities;

namespace StudentEvents.Tests.Services
{
    public class TokenServiceTests
    {
        [Fact]
        public void GenerateToken_ReturnsToken_WhenUserProvided()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var tokenService = new TokenService(config);
            var user = new User { Id = System.Guid.NewGuid(), Email = "t@t", DisplayName = "T" };

            var token = tokenService.GenerateToken(user);

            token.Should().NotBeNullOrEmpty();
        }
    }
}
