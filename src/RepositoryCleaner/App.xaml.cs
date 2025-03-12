namespace RepositoryCleaner
{
    using System.Windows;
    using System.Windows.Controls;
    using Catel.IoC;
    using Catel.Logging;
    using Orchestra;
    using RepositoryCleaner.Services;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
#if DEBUG
            LogManager.AddDebugListener(true);
#endif
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceLocator = ServiceLocator.Default;
            var applicationInitializationService = serviceLocator.ResolveRequiredType<IApplicationInitializationService>();
            await applicationInitializationService.InitializeAsync();

            this.ApplyTheme();

            // Show tooltips for 30 seconds
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(30 * 1000));
        }
    }
}
