namespace RepositoryCleaner.Services
{
    using System;
    using Models;

    internal class RepositoryEventArgs : EventArgs
    {
        public RepositoryEventArgs(Repository repository)
        {
            Repository = repository;
        }

        public Repository Repository { get; set; }
    }
}
