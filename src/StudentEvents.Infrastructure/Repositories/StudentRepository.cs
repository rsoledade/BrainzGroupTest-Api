using Microsoft.EntityFrameworkCore;
using StudentEvents.Domain.Entities;
using StudentEvents.Infrastructure.Data;

namespace StudentEvents.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly StudentEventsDbContext _dbContext;

        public StudentRepository(StudentEventsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _dbContext.Students.Include(s => s.Events).AsNoTracking().ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Students.Include(s => s.Events).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpsertStudentAsync(Student student)
        {
            var existing = await _dbContext.Students.FirstOrDefaultAsync(s => s.UserPrincipalName == student.UserPrincipalName);
            if (existing == null)
            {
                student.Id = Guid.NewGuid();
                _dbContext.Students.Add(student);
            }
            else
            {
                existing.DisplayName = student.DisplayName;
                existing.Mail = student.Mail;
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpsertEventsAsync(Guid studentId, IEnumerable<CalendarEvent> events)
        {
            var existingEvents = _dbContext.CalendarEvents.Where(e => e.StudentId == studentId);
            _dbContext.CalendarEvents.RemoveRange(existingEvents);
            foreach (var ev in events)
            {
                ev.Id = Guid.NewGuid();
                ev.StudentId = studentId;
                _dbContext.CalendarEvents.Add(ev);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}