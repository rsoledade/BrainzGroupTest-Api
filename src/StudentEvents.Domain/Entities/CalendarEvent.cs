using System;

namespace StudentEvents.Domain.Entities
{
 public class CalendarEvent
 {
 public Guid Id { get; set; }
 public string GraphId { get; set; } = string.Empty;
 public string Subject { get; set; } = string.Empty;
 public DateTimeOffset Start { get; set; }
 public DateTimeOffset End { get; set; }
 public string BodyPreview { get; set; } = string.Empty;

 public Guid StudentId { get; set; }
 public Student Student { get; set; } = null!;
 }
}