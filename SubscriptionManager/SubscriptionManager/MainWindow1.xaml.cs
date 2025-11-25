using System.Windows;
using SubscriptionManager.ViewModels;

namespace SubscriptionManager
{
    public partial class MainWindow1 : Window
    {
        public MainWindow1()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            var statsWindow = new Views.StatisticsWindow();
            statsWindow.Owner = this;
            statsWindow.ShowDialog();
        }
    }
}