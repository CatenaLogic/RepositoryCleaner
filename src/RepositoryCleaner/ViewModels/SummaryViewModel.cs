// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SummaryViewModel.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.Data;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Models;
    using Services;

    internal class SummaryViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly FastObservableCollection<Repository> _repositories;
        private readonly ChangeNotificationWrapper _changeNotificationWrapper;

        private bool _hasPendingUpdates;

        public SummaryViewModel(FastObservableCollection<Repository> repositories)
        {
            Argument.IsNotNull(() => repositories);

            _repositories = repositories;

            _changeNotificationWrapper = new ChangeNotificationWrapper(repositories);
        }

        public int RepositoriesToClean { get; private set; }

        public long TotalSize { get; private set; }

        public bool IsBusy { get; private set; }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _changeNotificationWrapper.CollectionChanged += OnRepositoriesChanged;
            _changeNotificationWrapper.CollectionItemPropertyChanged += OnRepositoryPropertyChanged;

            await Update();
        }

        protected override async Task CloseAsync()
        {
            _changeNotificationWrapper.CollectionChanged -= OnRepositoriesChanged;
            _changeNotificationWrapper.CollectionItemPropertyChanged -= OnRepositoryPropertyChanged;

            await base.CloseAsync();
        }

        private async void OnRepositoriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await Update();
        }

        private async void OnRepositoryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await Update();
        }

        private async Task Update()
        {
            if (IsBusy)
            {
                _hasPendingUpdates = true;
                return;
            }

            using (CreateIsBusyScope())
            {
                Log.Debug("Updating summary");

                var repositories = _repositories.Where(x => x.IsIncluded).ToList();

                RepositoriesToClean = repositories.Count;

                TotalSize = 0L;

                foreach (var repository in repositories)
                {
                    if (repository.CleanableSize.HasValue)
                    {
                        TotalSize += repository.CleanableSize.Value;
                    }
                }
            }

            if (_hasPendingUpdates)
            {
                _hasPendingUpdates = false;
                await Update();
            }
        }

        private IDisposable CreateIsBusyScope()
        {
            return new DisposableToken<SummaryViewModel>(this, x => x.Instance.IsBusy = true, x => x.Instance.IsBusy = false);
        }
    }
}