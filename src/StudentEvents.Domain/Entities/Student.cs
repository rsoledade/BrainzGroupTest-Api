using System;
using System.Collections.Generic;

namespace StudentEvents.Domain.Entities
{
 public class Student
 {
 public Guid Id { get; set; }
 public string DisplayName { get; set; } = string.Empty;
 public string Mail { get; set; } = string.Empty;
 public string UserPrincipalName { get; set; } = string.Empty;

 public ICollection<CalendarEvent> Events { get; set; } = new List<CalendarEvent>();
 }
}