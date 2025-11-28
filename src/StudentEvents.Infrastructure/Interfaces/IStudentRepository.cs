using StudentEvents.Domain.Entities;

namespace StudentEvents.Infrastructure.Repositories
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(Guid id);
        Task<IEnumerable<Student>> GetAllAsync();
        Task UpsertStudentAsync(Student student);
        Task UpsertEventsAsync(Guid studentId, IEnumerable<CalendarEvent> events);
    }
}
