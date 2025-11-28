using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StudentEvents.Application.Services;

namespace StudentEvents.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _service;

        public StudentsController(IStudentService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var listStudents = await _service.GetAllAsync();
            var response = listStudents.Select(student => new
            {
                id = student.Id,
                displayName = student.DisplayName,
                mail = student.Mail
            });
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var student = await _service.GetByIdAsync(id);

            if (student == null) return NotFound();

            var response = new
            {
                id = student.Id,
                displayName = student.DisplayName,
                mail = student.Mail,
                events = student.Events.Select(e => new { id = e.Id, subject = e.Subject, start = e.Start, end = e.End })
            };

            return Ok(response);
        }
    }
}
