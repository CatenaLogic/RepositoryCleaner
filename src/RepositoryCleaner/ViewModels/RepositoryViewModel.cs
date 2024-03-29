﻿namespace RepositoryCleaner.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using Catel.Fody;
    using Catel.MVVM;
    using Models;
    using Services;

    internal class RepositoryViewModel : ViewModelBase
    {
        private readonly ICleanerService _cleanerService;

        public RepositoryViewModel(Repository repository, ICleanerService cleanerService)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(cleanerService);

            Repository = repository;
            _cleanerService = cleanerService;
        }

        [Model(SupportIEditableObject = false)]
        [Expose("Name")]
        [Expose("Directory")]
        [Expose("IsIncluded")]
        public Repository Repository { get; private set; }

        public ulong CleanableSpace { get; private set; }

        public bool IsBusy { get; protected set; }

        protected override async Task InitializeAsync()
        {
            _cleanerService.RepositoryCleaning += OnCleanerServiceRepositoryCleaning;
            _cleanerService.RepositoryCleaned += OnCleanerServiceRepositoryCleaned;

            CleanableSpace = await Repository.CalculateCleanableSpaceAsync();
        }

        protected override async Task CloseAsync()
        {
            _cleanerService.RepositoryCleaning -= OnCleanerServiceRepositoryCleaning;
            _cleanerService.RepositoryCleaned -= OnCleanerServiceRepositoryCleaned;
        }

        private void OnCleanerServiceRepositoryCleaning(object sender, RepositoryEventArgs e)
        {
            if (ReferenceEquals(Repository, e.Repository))
            {
                IsBusy = true;
            }
        }

        private void OnCleanerServiceRepositoryCleaned(object sender, RepositoryEventArgs e)
        {
            if (ReferenceEquals(Repository, e.Repository))
            {
                IsBusy = false;
            }
        }
    }
}
