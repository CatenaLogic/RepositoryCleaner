namespace RepositoryCleaner.Services
{
    using System;
    using System.Threading.Tasks;
    using Catel.IoC;
    using Catel.Logging;

    public class ApplicationInitializationService : IApplicationInitializationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IServiceLocator _serviceLocator;
        private readonly ITypeFactory _typeFactory;

        public ApplicationInitializationService(ITypeFactory typeFactory, IServiceLocator serviceLocator)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);
            ArgumentNullException.ThrowIfNull(serviceLocator);

            _typeFactory = typeFactory;
            _serviceLocator = serviceLocator;
        }

        public async Task InitializeAsync()
        {

        }
    }
}
