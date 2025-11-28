using Xunit;
using FluentAssertions;
using Moq;
using StudentEvents.Api.Controllers;
using StudentEvents.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StudentEvents.Tests.Controllers
{
    public class AuthControllerLoginSuccessTests
    {
        [Fact]
        public async Task Login_ReturnsOk_WithToken()
        {
            var mockAuth = new Mock<IAuthService>();
            mockAuth.Setup(a => a.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("token-xyz");
            var controller = new AuthController(mockAuth.Object);
            var result = await controller.Login(new LoginRequest("x@x", "p"));
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            var token = ((dynamic)ok.Value).token as string;
            token.Should().Be("token-xyz");
        }
    }
}
