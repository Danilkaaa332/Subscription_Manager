using System.Windows;
using SubscriptionManager.Models;
using SubscriptionManager.ViewModels;

namespace SubscriptionManager.Views
{
    public partial class AddEditSubscriptionWindow : Window
    {
        public AddEditSubscriptionViewModel ViewModel { get; private set; }
        public AddEditSubscriptionWindow()
        {
            InitializeComponent();
            ViewModel = new AddEditSubscriptionViewModel();

            DataContext = ViewModel.Subscription;
        }
        public AddEditSubscriptionWindow(Subscription subscriptionToEdit)
        {
            InitializeComponent();
            ViewModel = new AddEditSubscriptionViewModel(subscriptionToEdit);

            DataContext = ViewModel.Subscription;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ViewModel.Subscription.ServiceName))
            {
                MessageBox.Show("Введите название сервиса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ViewModel.Subscription.Cost <= 0)
            {
                MessageBox.Show("Стоимость должна быть положительным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ViewModel.Subscription.NextBillingDate.Date < DateTime.Today)
            {
                MessageBox.Show("Дата не может быть в прошлом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
