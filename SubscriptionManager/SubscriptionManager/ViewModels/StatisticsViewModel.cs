using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using SubscriptionManager.Models;
using SubscriptionManager.Services;
using SubscriptionManager.Services.Repositories;
using LiveCharts;
using LiveCharts.Wpf;

namespace SubscriptionManager.ViewModels
{
    public class StatisticsViewModel : INotifyPropertyChanged
    {
        private readonly ISubscriptionService _service;

        public event PropertyChangedEventHandler? PropertyChanged;
        public decimal TotalMonthly { get; set; }
        public decimal TotalYearly { get; set; }
        public int ActiveCount { get; set; }

        public Dictionary<string, decimal> CategoryData { get; set; } = new Dictionary<string, decimal>();
        public List<MonthlyData> MonthlyExpenses { get; set; } = new List<MonthlyData>();

        private SeriesCollection? _pieSeries;
        public SeriesCollection PieSeries
        {
            get
            {
                if (_pieSeries == null)
                    InitializeCharts();
                return _pieSeries ?? new SeriesCollection();
            }
        }

        private SeriesCollection? _lineSeries;
        public SeriesCollection LineSeries
        {
            get
            {
                if (_lineSeries == null)
                    InitializeCharts();
                return _lineSeries ?? new SeriesCollection();
            }
        }

        public List<string> Months => MonthlyExpenses.Select(m => m.Month).ToList();
        public List<decimal> MonthlyAmounts => MonthlyExpenses.Select(m => m.Amount).ToList();

        public StatisticsViewModel() : this(new SubscriptionService(new SubscriptionRepository(new Data.AppDbContext()))) { }

        public StatisticsViewModel(ISubscriptionService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            LoadData();
        }

        private void LoadData()
        {
            var allSubscriptions = _service.GetAllSubscriptions()?.ToList() ?? new List<Subscription>();
            var activeSubscriptions = allSubscriptions.Where(s => s.Status == "Активна").ToList();

            TotalMonthly = activeSubscriptions.Sum(s => s.MonthlyCost);
            TotalYearly = activeSubscriptions.Sum(s => s.YearlyCost);
            ActiveCount = activeSubscriptions.Count;

            PrepareChartData(activeSubscriptions);
            PrepareMonthlyData(activeSubscriptions);
        }

        private void PrepareChartData(List<Subscription> activeSubscriptions)
        {
            CategoryData = activeSubscriptions.GroupBy(s => s.Category).ToDictionary(g => g.Key, g => g.Sum(s => s.MonthlyCost));
        }

        private void PrepareMonthlyData(List<Subscription> activeSubscriptions)
        {
            MonthlyExpenses.Clear();

            for (int i = 5; i >= 0; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                var monthStart = new DateTime(date.Year, date.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                decimal monthlyTotal = 0;

                foreach (var subscription in activeSubscriptions)
                {
                    if (HasBillingInMonth(subscription, date.Year, date.Month))
                    {
                        monthlyTotal += subscription.Cost; 
                    }
                }

                MonthlyExpenses.Add(new MonthlyData
                {
                    Month = date.ToString("MMM yyyy"),
                    Amount = Math.Round(monthlyTotal, 2)
                });
            }
        }

        private bool HasBillingInMonth(Subscription subscription, int year, int month)
        {
            var targetDate = new DateTime(year, month, 1);

            if (subscription.PaymentPeriod == "Ежемесячно")
                return true;

            if (subscription.PaymentPeriod == "Ежеквартально")
            {
                var monthsDifference = (targetDate.Year - subscription.NextBillingDate.Year) * 12 + targetDate.Month - subscription.NextBillingDate.Month;
                return monthsDifference % 3 == 0;
            }
            if (subscription.PaymentPeriod == "Ежегодно")
            {
                return targetDate.Month == subscription.NextBillingDate.Month && targetDate.Year >= subscription.NextBillingDate.Year;
            }

            return false;
        }

        private void InitializeCharts()
        {
            _pieSeries = new SeriesCollection();

            var colors = new[]
            {
                "#FF3366CC", 
                "#FF33CC66", 
                "#FFCC6633", 
                "#FFCC3366", 
                "#FF6633CC", 
                "#FF33CCCC"  
            };

            int colorIndex = 0;
            foreach (var category in CategoryData)
            {
                _pieSeries.Add(new PieSeries
                {
                    Title = category.Key,
                    Values = new ChartValues<decimal> { category.Value },
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y:F0} руб.",
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[colorIndex % colors.Length])),
                    Stroke = Brushes.White, 
                    StrokeThickness = 2
                });
                colorIndex++;
            }

            _lineSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Расходы по месяцам",
                    Values = new ChartValues<decimal>(MonthlyAmounts),
                    PointGeometry = DefaultGeometries.Circle,
                    Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3366CC")), 
                    Fill = Brushes.Transparent,
                    StrokeThickness = 3,
                    PointForeground = Brushes.White 
                }
            };

            Notify(nameof(PieSeries));
            Notify(nameof(LineSeries));
            Notify(nameof(Months));
        }

        protected void Notify(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}