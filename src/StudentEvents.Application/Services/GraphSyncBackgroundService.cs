using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace StudentEvents.Application.Services
{
    public class GraphSyncBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<GraphSyncBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

        public GraphSyncBackgroundService(IServiceProvider provider, ILogger<GraphSyncBackgroundService> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting Graph sync...");

                    using (var scope = _provider.CreateScope())
                    {
                        var sync = scope.ServiceProvider.GetRequiredService<IGraphSyncService>();
                        await sync.SyncAsync();
                    }

                    _logger.LogInformation("Graph sync completed.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Graph sync failed");
                }

                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // shutting down
                }
            }
        }
    }
}
