using Microsoft.EntityFrameworkCore;
using StudentEvents.Domain.Entities;

namespace StudentEvents.Infrastructure.Data
{
 public class StudentEventsDbContext : DbContext
 {
 public StudentEventsDbContext(DbContextOptions<StudentEventsDbContext> options) : base(options)
 {
 }

 public DbSet<Student> Students { get; set; } = null!;
 public DbSet<CalendarEvent> CalendarEvents { get; set; } = null!;
 public DbSet<User> Users { get; set; } = null!;
 }
}