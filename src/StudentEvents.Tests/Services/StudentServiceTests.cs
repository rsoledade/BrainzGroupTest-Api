using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StudentEvents.Infrastructure.Data;
using StudentEvents.Application.Services;
using StudentEvents.Domain.Entities;
using System.Threading.Tasks;

namespace StudentEvents.Tests.Services
{
    public class StudentServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsSeededStudents()
        {
            var options = new DbContextOptionsBuilder<StudentEventsDbContext>()
            .UseInMemoryDatabase("StudentTestDb")
            .Options;

            using var db = new StudentEventsDbContext(options);
            db.Students.Add(new Student { Id = System.Guid.NewGuid(), DisplayName = "S1", Mail = "s1@local", UserPrincipalName = "s1" });
            db.SaveChanges();

            var repository = new StudentEvents.Infrastructure.Repositories.StudentRepository(db);
            var service = new StudentService(repository);
            var all = await service.GetAllAsync();
            all.Should().HaveCount(1);
        }
    }
}
