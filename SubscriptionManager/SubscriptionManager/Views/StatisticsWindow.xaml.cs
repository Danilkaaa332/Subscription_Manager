using System.Windows;

namespace SubscriptionManager.Views
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow()
        {
            InitializeComponent();
            DataContext = new ViewModels.StatisticsViewModel();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}