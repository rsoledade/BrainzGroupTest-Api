using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StudentEvents.Infrastructure.Repositories;

namespace StudentEvents.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _repo;

        public StudentsController(IStudentRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            var res = list.Select(s => new
            {
                id = s.Id,
                displayName = s.DisplayName,
                mail = s.Mail
            });
            return Ok(res);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var s = await _repo.GetByIdAsync(id);
            if (s == null) return NotFound();
            var res = new
            {
                id = s.Id,
                displayName = s.DisplayName,
                mail = s.Mail,
                events = s.Events.Select(e => new { id = e.Id, subject = e.Subject, start = e.Start, end = e.End })
            };
            return Ok(res);
        }
    }
}
