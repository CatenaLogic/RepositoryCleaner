﻿namespace RepositoryCleaner.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;
    using Humanizer;
    using Models;

    internal class SizeToolTipViewModel : ViewModelBase
    {
        private readonly Repository _repository;

        public SizeToolTipViewModel(Repository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);

            _repository = repository;

            Items = new ObservableCollection<string>();
        }

        public bool IsBusy { get; private set; }

        public ObservableCollection<string> Items { get; private set; }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            await UpdateAsync();
        }

        private async Task UpdateAsync()
        {
            using (CreateIsBusyScope())
            {
                Items.Clear();

                var cleanContext = new CleanContext(_repository);

                foreach (var cleaner in _repository.Cleaners)
                {
                    var size = await Task.Run(() => cleaner.CalculateCleanableSpace(cleanContext));
                    var line = $"{cleaner.Name} => {((size == 0L) ? "0 bytes" : ((long)size).Bytes().Humanize("#.#"))}";

                    Items.Add(line);
                }
            }
        }

        private IDisposable CreateIsBusyScope()
        {
            return new DisposableToken<SizeToolTipViewModel>(this, x => x.Instance.IsBusy = true, x => x.Instance.IsBusy = false);
        }
    }
}
