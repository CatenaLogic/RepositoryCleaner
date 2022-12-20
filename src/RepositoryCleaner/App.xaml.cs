namespace RepositoryCleaner
{
    using System.Windows;
    using System.Windows.Controls;
    using Catel.Logging;
    using Orchestra;

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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.ApplyTheme();

            // Show tooltips for 30 seconds
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(30 * 1000));
        }
    }
}
