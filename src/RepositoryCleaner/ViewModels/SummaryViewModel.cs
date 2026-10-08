namespace RepositoryCleaner.ViewModels
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.Data;
    using Catel.MVVM;
    using Catel.Services;
    using Microsoft.Extensions.Logging;
    using Models;

    internal class SummaryViewModel : ViewModelBase
    {
        private readonly ILogger<SummaryViewModel> _logger;
        private readonly FastObservableCollection<Repository> _repositories;
        private readonly IDispatcherService _dispatcherService;
        private readonly ChangeNotificationWrapper _changeNotificationWrapper;

        private bool _hasPendingUpdates;

        public SummaryViewModel(FastObservableCollection<Repository> repositories, IServiceProvider serviceProvider, ILogger<SummaryViewModel> logger, IDispatcherService dispatcherService)
            : base(serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(repositories);
            ArgumentNullException.ThrowIfNull(dispatcherService);

            _logger = logger;
            _repositories = repositories;
            _dispatcherService = dispatcherService;

            _changeNotificationWrapper = new ChangeNotificationWrapper(repositories);
        }

        public int RepositoriesToClean { get; private set; }

        public ulong TotalSize { get; private set; }

        public bool IsBusy { get; private set; }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _changeNotificationWrapper.CollectionChanged += OnRepositoriesChanged;
            _changeNotificationWrapper.CollectionItemPropertyChanged += OnRepositoryPropertyChanged;

            await UpdateAsync();
        }

        protected override async Task CloseAsync()
        {
            _changeNotificationWrapper.CollectionChanged -= OnRepositoriesChanged;
            _changeNotificationWrapper.CollectionItemPropertyChanged -= OnRepositoryPropertyChanged;

            await base.CloseAsync();
        }

        private async void OnRepositoriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await UpdateAsync();
        }

        private async void OnRepositoryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _dispatcherService.BeginInvoke(async () => await UpdateAsync());
        }

        private async Task UpdateAsync()
        {
            if (IsBusy)
            {
                _hasPendingUpdates = true;
                return;
            }

            using (CreateIsBusyScope())
            {
                _logger.LogDebug("Updating summary");

                var repositories = _repositories.Where(x => x.IsIncluded).ToList();

                RepositoriesToClean = repositories.Count;

                ulong totalSize = 0L;

                foreach (var repository in repositories)
                {
                    if (repository.CleanableSize.HasValue)
                    {
                        totalSize += repository.CleanableSize.Value;
                    }
                }

                TotalSize = totalSize;

                _logger.LogDebug("Updated summary");
            }

            if (_hasPendingUpdates)
            {
                _hasPendingUpdates = false;
                await UpdateAsync();
            }
        }

        private IDisposable CreateIsBusyScope()
        {
            return new DisposableToken<SummaryViewModel>(this, x => x.Instance.IsBusy = true, x => x.Instance.IsBusy = false);
        }
    }
}
