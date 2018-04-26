// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Windows;
    using Catel.Windows.Threading;
    using Orchestra;
    using Services;
    using Views;
    using AssemblyHelper = Orchestra.AssemblyHelper;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ThemeHelper.EnsureApplicationThemes(AssemblyHelper.GetEntryAssembly(), true);
            //StyleHelper.CreateStyleForwardersForDefaultStyles();

#if DEBUG
            LogManager.AddDebugListener(true);
#endif

            // Show tooltips for 30 seconds
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(30 * 1000));

            var dependencyResolver = this.GetDependencyResolver();
            var applicationInitializationService = dependencyResolver.Resolve<IApplicationInitializationService>();
            var task = applicationInitializationService.Initialize();
            task.ContinueWith(x => Dispatcher.BeginInvoke(ContinueStartup));
        }

        private void ContinueStartup()
        {
            //var assembly = AssemblyHelper.GetEntryAssembly();
            //var logFileName = Path.Combine(assembly.GetDirectory(), string.Format("RepositoryCleaner_{0}.log", DateTime.Now.ToString("yyyyMMdd_HHmmss")));
            //var fileLogListener = new FileLogListener(logFileName, 25 * 1024);

            //LogManager.AddListener(fileLogListener);

            var shell = new MainWindow();
            shell.Show();
        }
    }
}