using StudentEvents.Application.Services;
using StudentEvents.Infrastructure.Repositories;

namespace StudentEvents.Api.Configuration
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IGraphSyncService, GraphSyncService>();
            services.AddSingleton<IGraphClientFactory, GraphClientFactory>();
            services.AddHostedService<GraphSyncBackgroundService>();
            services.AddSingleton<DatabaseInitializer>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
