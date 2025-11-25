using System;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManager.Models
{
    public class SubscriptionHistory
    {
        [Key]
        public int Id { get; set; }

        public int SubscriptionId { get; set; }
        public DateTime Date { get; set; }
        public decimal MonthlyCost { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RecordedAt { get; set; } = DateTime.Now;
    }
}