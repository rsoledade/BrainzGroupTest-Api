using StudentEvents.Domain.Entities;
using StudentEvents.Infrastructure.Repositories;

namespace StudentEvents.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Student>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<Student?> GetByIdAsync(Guid id) => await _repository.GetByIdAsync(id);
        public async Task UpsertStudentAsync(Student student) => await _repository.UpsertStudentAsync(student);
        public async Task UpsertEventsAsync(Guid studentId, IEnumerable<CalendarEvent> events) => await _repository.UpsertEventsAsync(studentId, events);
    }
}
