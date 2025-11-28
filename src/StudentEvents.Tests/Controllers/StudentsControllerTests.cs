using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using StudentEvents.Api.Controllers;
using StudentEvents.Application.Services;
using Moq;
using StudentEvents.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentEvents.Tests.Controllers
{
    public class StudentsControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkWithStudents()
        {
            var mockService = new Mock<IStudentService>();
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Student> { new Student { Id = System.Guid.NewGuid(), DisplayName = "S1", Mail = "s1@local" } });
            var controller = new StudentsController(mockService.Object);
            var result = await controller.GetAll();
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
        }
    }
}
