using StudentEvents.Domain.Entities;

namespace StudentEvents.Application.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(Guid id);
        Task UpsertStudentAsync(Student student);
        Task UpsertEventsAsync(Guid studentId, IEnumerable<CalendarEvent> events);
    }
}
