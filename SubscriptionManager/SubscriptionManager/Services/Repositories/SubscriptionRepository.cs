using System.Collections.Generic;
using System.Linq;
using SubscriptionManager.Data;
using SubscriptionManager.Models;

namespace SubscriptionManager.Services.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _context;

        public SubscriptionRepository(AppDbContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        public IEnumerable<Subscription> GetAll()
        {
            return _context.Subscriptions.ToList();
        }

        public Subscription GetById(int id)
        {
            return _context.Subscriptions.Find(id);
        }

        public void Add(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
        }

        public void Update(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
        }

        public void Remove(Subscription subscription)
        {
            _context.Subscriptions.Remove(subscription);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
