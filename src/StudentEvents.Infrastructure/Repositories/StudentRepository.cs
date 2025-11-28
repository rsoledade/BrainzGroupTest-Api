using Microsoft.EntityFrameworkCore;
using StudentEvents.Domain.Entities;
using StudentEvents.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEvents.Infrastructure.Repositories
{
 public class StudentRepository : IStudentRepository
 {
 private readonly StudentEventsDbContext _db;
 public StudentRepository(StudentEventsDbContext db)
 {
 _db = db;
 }

 public async Task<IEnumerable<Student>> GetAllAsync()
 {
 return await _db.Students.Include(s => s.Events).AsNoTracking().ToListAsync();
 }

 public async Task<Student?> GetByIdAsync(Guid id)
 {
 return await _db.Students.Include(s => s.Events).FirstOrDefaultAsync(s => s.Id == id);
 }

 public async Task UpsertStudentAsync(Student student)
 {
 var existing = await _db.Students.FirstOrDefaultAsync(s => s.UserPrincipalName == student.UserPrincipalName);
 if (existing == null)
 {
 student.Id = Guid.NewGuid();
 _db.Students.Add(student);
 }
 else
 {
 existing.DisplayName = student.DisplayName;
 existing.Mail = student.Mail;
 }
 await _db.SaveChangesAsync();
 }

 public async Task UpsertEventsAsync(Guid studentId, IEnumerable<CalendarEvent> events)
 {
 var existingEvents = _db.CalendarEvents.Where(e => e.StudentId == studentId);
 _db.CalendarEvents.RemoveRange(existingEvents);
 foreach (var ev in events)
 {
 ev.Id = Guid.NewGuid();
 ev.StudentId = studentId;
 _db.CalendarEvents.Add(ev);
 }
 await _db.SaveChangesAsync();
 }
 }
}