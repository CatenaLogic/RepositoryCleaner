namespace RepositoryCleaner.Tests
{
    using Catel;
    using Catel.MVVM;
    using Microsoft.Extensions.DependencyInjection;

    internal static class ServiceCollectionHelper
    {
        public static IServiceCollection CreateServiceCollection()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddCatelCore();
            services.AddCatelMvvm();

            return services;
        }
    }
}
