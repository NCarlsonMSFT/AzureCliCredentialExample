using Azure.Identity;
using Azure.ResourceManager;

namespace AzureCliCredentialExample
{
    public class Worker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Worker> _logger;

        public Worker(IConfiguration configuration, ILogger<Worker> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var creds = new DefaultAzureCredential(includeInteractiveCredentials: false);
            bool connected = false;

            while (!connected)
            {
                try
                {
                    ArmClient client = new ArmClient(new DefaultAzureCredential());
                    var subscriptionResource = await client.GetDefaultSubscriptionAsync();
                    var subscription = await subscriptionResource.GetAsync();
                    _logger.LogCritical($"Displayname: {subscription.Value.Data.DisplayName}");
                    connected = true;
                }
                catch (CredentialUnavailableException ex)
                {
                    _logger.LogWarning("Could not authenticate");
                    await Task.Delay(1000);
                }
            }
        }
    }
}