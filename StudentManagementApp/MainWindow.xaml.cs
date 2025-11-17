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

            // This one line connects your View (XAML) to your ViewModel (C# Logic)
            DataContext = new ViewModels.MainViewModel();
        }
    }
}