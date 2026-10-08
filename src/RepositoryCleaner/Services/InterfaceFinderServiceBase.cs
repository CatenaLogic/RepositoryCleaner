namespace RepositoryCleaner.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public abstract class InterfaceFinderServiceBase<TInterface>
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(InterfaceFinderServiceBase<TInterface>));

        private readonly List<TInterface> _implementations;

        protected InterfaceFinderServiceBase(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);

            _implementations = serviceProvider.GetServices<TInterface>().ToList();
            foreach (var implementation in _implementations)
            {
                Logger.LogDebug("Found implementation {ImplementationType}", implementation.GetType().Name);
            }
        }

        protected IEnumerable<TInterface> GetAvailableItems()
        {
            return _implementations.ToList();
        }
    }
}
