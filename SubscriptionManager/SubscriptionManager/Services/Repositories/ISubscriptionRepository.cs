using System.Collections.Generic;
using SubscriptionManager.Models;

namespace SubscriptionManager.Services.Repositories
{
    public interface ISubscriptionRepository
    {
        IEnumerable<Subscription> GetAll();
        Subscription GetById(int id);
        void Add(Subscription subscription);
        void Update(Subscription subscription);
        void Remove(Subscription subscription);
        void SaveChanges();
    }
}
