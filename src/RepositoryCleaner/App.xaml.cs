namespace RepositoryCleaner
{
    using System.Windows;
    using System.Windows.Controls;
    using Catel;
    using Catel.IoC;
    using Catel.MVVM;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Orc;
    using Orc.FileSystem;
    using Orchestra;
    using RepositoryCleaner.Cleaners;
    using RepositoryCleaner.Services;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
#pragma warning disable IDISP006
        private readonly IHost _host;
#pragma warning restore IDISP006

        public App()
        {
            _host = new HostBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddCatelCore();
                    services.AddCatelMvvm();

                    services.AddOrcControls();
                    services.AddOrcFileSystem();
                    services.AddOrcLogViewer();
                    services.AddOrcNotifications();
                    services.AddOrcSerializationJson();
                    services.AddOrcSystemInfo();
                    services.AddOrcTheming();

                    services.AddOrchestraCore();

                    services.AddLogging(loggingBuilder => loggingBuilder.AddDebug());

                    services.AddTransient<ICleaner, CakeToolsCleaner>();
                    services.AddTransient<ICleaner, EmptyDirectoryCleaner>();
                    services.AddTransient<ICleaner, IntermediateDirectoryCleaner>();
                    services.AddTransient<ICleaner, NuGetPackagesInLibFolderCleaner>();
                    services.AddTransient<ICleaner, OutputDirectoryCleaner>();
                    services.AddTransient<ICleaner, OutputDirectoryInRootCleaner>();
                    services.AddSingleton<ICleanerService, CleanerService>();
                    services.AddSingleton<IApplicationInitializationService, ApplicationInitializationService>();
                    services.AddSingleton<IRepositoryService, RepositoryService>();
                })
                .Build();

            IoCContainer.ServiceProvider = _host.Services;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceProvider = _host.Services;
            var applicationInitializationService = serviceProvider.GetRequiredService<IApplicationInitializationService>();
            await applicationInitializationService.InitializeAsync();
            serviceProvider.CreateTypesThatMustBeConstructedAtStartup();

            this.ApplyTheme();

            // Show tooltips for 30 seconds
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(30 * 1000));
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }
    }
}
