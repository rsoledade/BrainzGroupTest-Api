using StudentEvents.Domain.Entities;

namespace StudentEvents.Application.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
