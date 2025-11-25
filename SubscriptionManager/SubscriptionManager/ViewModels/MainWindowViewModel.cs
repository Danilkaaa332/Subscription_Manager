using SubscriptionManager.Models;
using SubscriptionManager.Services;
using SubscriptionManager.Services.Repositories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using SubscriptionManager.Views;
using System.Windows;

namespace SubscriptionManager.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ISubscriptionService _service;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Subscription> Subscriptions { get; set; }
        public ICollectionView FilteredSubscriptions { get; set; }

        private Subscription? _selectedSubscription;
        public Subscription? SelectedSubscription
        {
            get => _selectedSubscription;
            set
            {
                _selectedSubscription = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSubscription)));

                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (SuspendCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _filterStatus = "Все";
        public string FilterStatus
        {
            get => _filterStatus;
            set
            {
                _filterStatus = value;
                FilteredSubscriptions.Refresh();
                UpdateSummary();
            }
        }

        private string _filterCategory = "Все";
        public string FilterCategory
        {
            get => _filterCategory;
            set
            {
                _filterCategory = value;
                FilteredSubscriptions.Refresh();
                UpdateSummary();
            }
        }

        private string _filterPeriod = "Все";
        public string FilterPaymentPeriod
        {
            get => _filterPeriod;
            set
            {
                _filterPeriod = value;
                FilteredSubscriptions.Refresh();
                UpdateSummary();
            }
        }

        private decimal _monthlyCost;
        public decimal MonthlyCost
        {
            get => _monthlyCost;
            set { _monthlyCost = value; Notify(nameof(MonthlyCost)); }
        }

        private decimal _yearlyCost;
        public decimal YearlyCost
        {
            get => _yearlyCost;
            set { _yearlyCost = value; Notify(nameof(YearlyCost)); }
        }

        private int _activeSubscriptionsCount;
        public int ActiveSubscriptionsCount
        {
            get => _activeSubscriptionsCount;
            set { _activeSubscriptionsCount = value; Notify(nameof(ActiveSubscriptionsCount)); }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SuspendCommand { get; }
        public ICommand CancelCommand { get; }

        public MainWindowViewModel(): this(new SubscriptionService(new SubscriptionRepository(new Data.AppDbContext()))){ }

        public MainWindowViewModel(ISubscriptionService service)
        {
            _service = service;

            Subscriptions = new ObservableCollection<Subscription>(_service.GetAllSubscriptions());
            FilteredSubscriptions = CollectionViewSource.GetDefaultView(Subscriptions);
            FilteredSubscriptions.Filter = FilterSubscription;

            AddCommand = new RelayCommand(AddSubscription!);
            EditCommand = new RelayCommand(EditSubscription!, CanEditSubscription);
            SuspendCommand = new RelayCommand(SuspendSubscription!, CanEditSubscription);
            CancelCommand = new RelayCommand(DeleteSubscription!, CanEditSubscription);

            UpdateSummary();
        }

        private bool FilterSubscription(object item)
        {
            if (item is not Subscription sub)
                return false;

            bool matchStatus = FilterStatus == "Все" || sub.Status == FilterStatus;
            bool matchCategory = FilterCategory == "Все" || sub.Category == FilterCategory;
            bool matchPeriod = FilterPaymentPeriod == "Все" || sub.PaymentPeriod == FilterPaymentPeriod;
            return matchStatus && matchCategory && matchPeriod;
        }

        private void UpdateSummary()
        {
            var filtered = FilteredSubscriptions.Cast<Subscription>().ToList();

            MonthlyCost = filtered.Sum(s => s.MonthlyCost);
            YearlyCost = filtered.Sum(s => s.YearlyCost);
            ActiveSubscriptionsCount = filtered.Count(s => s.Status == "Активна");
        }

        private void AddSubscription(object? obj)
        {
            var window = new AddEditSubscriptionWindow();
            if (window.ShowDialog() == true)
            {
                var added = window.ViewModel.Subscription;
                _service.AddSubscription(added);
                Subscriptions.Add(added);
                FilteredSubscriptions.Refresh();
                UpdateSummary();
            }
        }

        private bool CanEditSubscription(object? obj)
        {
            return SelectedSubscription != null;
        }

        private void EditSubscription(object? obj)
        {
            if (SelectedSubscription == null) return;

            var clone = SelectedSubscription.Clone();

            var window = new AddEditSubscriptionWindow(clone);
            if (window.ShowDialog() == true)
            {
                var updated = window.ViewModel.Subscription;

                SelectedSubscription.CopyFrom(updated);
                _service.Save();

                FilteredSubscriptions.Refresh();
                UpdateSummary();
            }
        }

        private void SuspendSubscription(object? obj)
        {
            if (SelectedSubscription == null)
                return;

            _service.UpdateSubscriptionStatus(SelectedSubscription.Id, "Приостановлена");
            SelectedSubscription.Status = "Приостановлена";

            FilteredSubscriptions.Refresh();
            UpdateSummary();
        }

        private void DeleteSubscription(object? obj)
        {
            if (SelectedSubscription == null)
                return;

            var result = System.Windows.MessageBox.Show(
                $"Вы уверены, что хотите полностью удалить подписку \"{SelectedSubscription.ServiceName}\"?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var subscriptionToRemove = SelectedSubscription;

                _service.RemoveSubscription(subscriptionToRemove.Id);

                Subscriptions.Remove(subscriptionToRemove);

                SelectedSubscription = null;

                FilteredSubscriptions.Refresh();
                UpdateSummary();

                System.Windows.MessageBox.Show(
                    "Подписка успешно удалена",
                    "Удаление завершено",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void Notify(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}