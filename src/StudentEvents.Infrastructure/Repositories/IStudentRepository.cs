using StudentEvents.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentEvents.Infrastructure.Repositories
{
 public interface IStudentRepository
 {
 Task<IEnumerable<Student>> GetAllAsync();
 Task<Student?> GetByIdAsync(Guid id);
 Task UpsertStudentAsync(Student student);
 Task UpsertEventsAsync(Guid studentId, IEnumerable<CalendarEvent> events);
 }
}