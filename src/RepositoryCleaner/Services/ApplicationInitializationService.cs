namespace RepositoryCleaner.Services
{
    using System;
    using System.Threading.Tasks;
    using Catel.Configuration;

    public class ApplicationInitializationService : IApplicationInitializationService
    {
        private readonly IConfigurationService _configurationService;

        public ApplicationInitializationService(IConfigurationService configurationService)
        {
            ArgumentNullException.ThrowIfNull(configurationService);
            _configurationService = configurationService;
        }

        public async Task InitializeAsync()
        {
            await _configurationService.LoadAsync();
        }
    }
}
