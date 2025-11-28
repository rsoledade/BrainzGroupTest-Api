namespace StudentEvents.Application.Services
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(string email, string password);
    }
}
