using System.Collections.Generic;
using SubscriptionManager.Models;

namespace SubscriptionManager.Services
{
    public interface ISubscriptionService
    {
        IEnumerable<Subscription> GetAllSubscriptions();
        IEnumerable<Subscription> GetActiveSubscriptions();
        IEnumerable<Subscription> GetSubscriptionsByCategory(string category);

        void AddSubscription(Subscription subscription);
        void UpdateSubscriptionStatus(int id, string newStatus);
        void RemoveSubscription(int id); 
        void Save();
    }
}