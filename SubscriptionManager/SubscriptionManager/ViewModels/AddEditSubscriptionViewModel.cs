using System.ComponentModel;
using SubscriptionManager.Models;

namespace SubscriptionManager.ViewModels
{
    public class AddEditSubscriptionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Subscription Subscription { get; set; }

        public AddEditSubscriptionViewModel()
        {
            Subscription = new Subscription
            {
                PaymentPeriod = "Ежемесячно",
                Category = "Другое",
                Status = "Активна",
                NextBillingDate = DateTime.Now.Date.AddDays(1)
            };
        }

        public AddEditSubscriptionViewModel(Subscription existing)
        {
            Subscription = existing?.Clone() ?? new Subscription();
        }

        protected void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}