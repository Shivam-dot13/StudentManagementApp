using System;
using System.Configuration;
using System.Data;
using System.Windows;

namespace StudentManagementApp
{
    public partial class App : Application
    {
        public App()
        {
            // This is the global WPF error handler
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // This is the global .NET error handler (our new one)
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;

            // Initialize the main window AFTER handlers are set
            this.Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        { 
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void ShowErrorMessage(string title, Exception ex)
        {
            string errorMessage = $"Fatal Error: {ex.Message}\n\n";
            if (ex.InnerException != null)
            {
                errorMessage += $"Inner Exception: {ex.InnerException.Message}\n\n";
            }
            errorMessage += ex.StackTrace;

            MessageBox.Show(errorMessage, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ShowErrorMessage("Dispatcher Unhandled Exception", e.Exception);
            e.Handled = true;
            Application.Current.Shutdown();
        }

        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // e.ExceptionObject is not always a .NET 'Exception'
            if (e.ExceptionObject is Exception ex)
            {
                ShowErrorMessage("AppDomain Unhandled Exception", ex);
            }
            Application.Current.Shutdown();
        }
    }
}