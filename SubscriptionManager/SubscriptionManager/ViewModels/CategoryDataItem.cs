namespace SubscriptionManager.ViewModels
{
    public class CategoryDataItem
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Color { get; set; } = string.Empty;
        public string AmountString => $"{Amount:F2} руб.";
    }
}