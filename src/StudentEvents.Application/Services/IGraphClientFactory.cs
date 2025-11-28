namespace StudentEvents.Application.Services
{
    public interface IGraphClientFactory
    {
        HttpClient Create();
    }
}
