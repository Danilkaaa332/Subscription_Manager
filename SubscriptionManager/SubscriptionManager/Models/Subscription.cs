using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace SubscriptionManager.Models
{
    public class Subscription : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [Key]
        public int Id { get; set; }

        private string _serviceName = string.Empty;
        public string ServiceName
        {
            get => _serviceName;
            set { _serviceName = value; OnPropertyChanged(); }
        }

        private string _category = "Другое";
        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(); }
        }

        private decimal _cost;
        public decimal Cost
        {
            get => _cost;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Стоимость не может быть отрицательной");

                _cost = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MonthlyCost));
                OnPropertyChanged(nameof(YearlyCost));
            }
        }

        private string _paymentPeriod = "Ежемесячно";
        public string PaymentPeriod
        {
            get => _paymentPeriod;
            set { _paymentPeriod = value; OnPropertyChanged(); OnPropertyChanged(nameof(MonthlyCost)); OnPropertyChanged(nameof(YearlyCost)); }
        }

        private DateTime _nextBillingDate = DateTime.Now.AddMonths(1);
        public DateTime NextBillingDate
        {
            get => _nextBillingDate;
            set { _nextBillingDate = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsUpcoming)); }
        }

        private string _managementUrl = string.Empty;
        public string ManagementUrl
        {
            get => _managementUrl;
            set { _managementUrl = value; OnPropertyChanged(); }
        }

        private string _notes = string.Empty;
        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(); }
        }

        private DateTime _addedDate = DateTime.Now;
        public DateTime AddedDate
        {
            get => _addedDate;
            set { _addedDate = value; OnPropertyChanged(); }
        }

        private string _status = "Активна";
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public decimal MonthlyCost
        {
            get
            {
                return PaymentPeriod switch
                {
                    "Ежемесячно" => Cost,
                    "Ежеквартально" => Decimal.Round(Cost / 3M, 2),
                    "Ежегодно" => Decimal.Round(Cost / 12M, 2),
                    _ => Cost
                };
            }
        }

        [NotMapped]
        public decimal YearlyCost
        {
            get
            {
                return PaymentPeriod switch
                {
                    "Ежемесячно" => Cost * 12M,
                    "Ежеквартально" => Cost * 4M,
                    "Ежегодно" => Cost,
                    _ => Cost
                };
            }
        }

        [NotMapped]
        public bool IsUpcoming
        {
            get
            {
                var now = DateTime.Now.Date;
                var next = NextBillingDate.Date;
                var diff = (next - now).TotalDays;
                return diff >= 0 && diff <= 3;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Subscription Clone()
        {
            return new Subscription
            {
                Id = this.Id,
                ServiceName = this.ServiceName,
                Category = this.Category,
                Cost = this.Cost,
                PaymentPeriod = this.PaymentPeriod,
                NextBillingDate = this.NextBillingDate,
                ManagementUrl = this.ManagementUrl,
                Notes = this.Notes,
                Status = this.Status
            };
        }

        public void CopyFrom(Subscription? other)
        {
            if (other == null) return;

            ServiceName = other.ServiceName;
            Category = other.Category;
            Cost = other.Cost;
            PaymentPeriod = other.PaymentPeriod;
            NextBillingDate = other.NextBillingDate;
            ManagementUrl = other.ManagementUrl;
            Notes = other.Notes;
            Status = other.Status;
        }
    }
}