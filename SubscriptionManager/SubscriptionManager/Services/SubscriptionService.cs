using System;
using System.Collections.Generic;
using System.Linq;
using SubscriptionManager.Models;
using SubscriptionManager.Services.Repositories;

namespace SubscriptionManager.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _repository;

        public SubscriptionService(ISubscriptionRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Subscription> GetAllSubscriptions()
        {
            return _repository.GetAll();
        }

        public IEnumerable<Subscription> GetActiveSubscriptions()
        {
            return _repository.GetAll().Where(s => s.Status == "Активна");
        }

        public IEnumerable<Subscription> GetSubscriptionsByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category) || category == "Все")
                return _repository.GetAll();

            return _repository.GetAll().Where(s => s.Category == category);
        }

        public void AddSubscription(Subscription subscription)
        {
            _repository.Add(subscription);
            _repository.SaveChanges();
        }

        public void UpdateSubscriptionStatus(int id, string newStatus)
        {
            var subscription = _repository.GetById(id);

            if (subscription == null)
                throw new InvalidOperationException("Subscription not found");

            subscription.Status = newStatus;

            _repository.Update(subscription);
            _repository.SaveChanges();
        }

        public void RemoveSubscription(int id)
        {
            var subscription = _repository.GetById(id);

            if (subscription == null)
                throw new InvalidOperationException("Subscription not found");

            _repository.Remove(subscription);
            _repository.SaveChanges();
        }

        public void Save()
        {
            _repository.SaveChanges();
        }
    }
}