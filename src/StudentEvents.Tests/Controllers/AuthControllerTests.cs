using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using StudentEvents.Api.Controllers;
using StudentEvents.Application.Services;
using Moq;
using System.Threading.Tasks;

namespace StudentEvents.Tests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenInvalid()
        {
            var mockAuth = new Mock<IAuthService>();
            mockAuth.Setup(a => a.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string?)null);
            var controller = new AuthController(mockAuth.Object);
            var result = await controller.Login(new LoginRequest("x@x", "p"));
            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}
