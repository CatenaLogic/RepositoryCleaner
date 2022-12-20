namespace RepositoryCleaner.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;

    public abstract class InterfaceFinderServiceBase<TInterface>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly List<TInterface> _implementations = new List<TInterface>();

        protected InterfaceFinderServiceBase()
        {
            var typeFactory = this.GetTypeFactory();

            var types = TypeCache.GetTypes(x => x.ImplementsInterfaceEx<TInterface>() && !x.IsAbstractEx());
            foreach (var type in types)
            {
                try
                {
                    Log.Debug("Found type '{0}'", type.Name);

                    _implementations.Add((TInterface) typeFactory.CreateInstance(type));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to instantiate '{0}'", type.FullName);
                }
            }
        }

        protected IEnumerable<TInterface> GetAvailableItems()
        {
            return _implementations.ToList();
        }
    }
}
