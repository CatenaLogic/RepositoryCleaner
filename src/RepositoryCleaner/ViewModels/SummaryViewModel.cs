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
        private readonly ICleanerService _cleanerService;
        private readonly IDispatcherService _dispatcherService;
        private readonly ChangeNotificationWrapper _changeNotificationWrapper;

        private bool _hasPendingUpdates;

        public SummaryViewModel(FastObservableCollection<Repository> repositories, ICleanerService cleanerService,
            IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(() => repositories);
            Argument.IsNotNull(() => cleanerService);
            Argument.IsNotNull(() => dispatcherService);

            _repositories = repositories;
            _cleanerService = cleanerService;
            _dispatcherService = dispatcherService;

            _changeNotificationWrapper = new ChangeNotificationWrapper(repositories);
        }

        public int RepositoriesToClean { get; private set; }

        public long TotalSize { get; private set; }

        public bool IsBusy { get; private set; }

        protected override async Task Initialize()
        {
            await base.Initialize();

            _changeNotificationWrapper.CollectionChanged += OnRepositoriesChanged;
            _changeNotificationWrapper.CollectionItemPropertyChanged += OnRepositoryPropertyChanged;

            await Update();
        }

        /// <summary>
        /// Closes this instance. Always called after the <see cref="M:Catel.MVVM.ViewModelBase.Cancel"/> of <see cref="M:Catel.MVVM.ViewModelBase.Save"/> method.
        /// </summary>
        protected override async Task Close()
        {
            _changeNotificationWrapper.CollectionChanged -= OnRepositoriesChanged;
            _changeNotificationWrapper.CollectionItemPropertyChanged -= OnRepositoryPropertyChanged;

            await base.Close();
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
                    TotalSize += await repository.CalculateCleanableSpaceAsync();
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