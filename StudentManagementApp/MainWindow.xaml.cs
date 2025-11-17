using System.Windows;

namespace StudentManagementApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                DataContext = new ViewModels.MainViewModel();
            }
            // ...and 'catch' any error that happens.
            catch (Exception ex)
            {
                // Show the error in a popup so we can read it!
                MessageBox.Show($"Fatal error on startup: {ex.Message}\n\n{ex.StackTrace}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Close the app
                Application.Current.Shutdown();
            }
            // This one line connects your View (XAML) to your ViewModel (C# Logic)
            DataContext = new ViewModels.MainViewModel();
        }
    }
}