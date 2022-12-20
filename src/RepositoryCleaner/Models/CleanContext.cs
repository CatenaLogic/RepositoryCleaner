namespace RepositoryCleaner.Models
{
    using System;
    using Catel;

    public class CleanContext
    {
        public CleanContext(Repository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);

            Repository = repository;
        }

        public Repository Repository { get; private set; }

        public bool IsDryRun { get; set; }
    }
}
