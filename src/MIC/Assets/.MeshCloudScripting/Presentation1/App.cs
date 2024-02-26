namespace CloudScripting.Sample
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Mesh.CloudScripting;

    public class App : IHostedService, IAsyncDisposable
    {
        private readonly ILogger<App> _logger;
        private readonly ICloudApplication _app;

        public App(ICloudApplication app, ILogger<App> logger)
        {
            _app = app;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task StartAsync(CancellationToken token)
        {
            // Add your app startup code here
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken token)
        {
            // Custom logic could be added here for user apps
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await StopAsync(CancellationToken.None)
                .ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }
    }
}