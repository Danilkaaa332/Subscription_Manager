using System.Collections.Generic;
using System.Linq;
using Moq;
using SubscriptionManager.Models;
using SubscriptionManager.Services;
using SubscriptionManager.Services.Repositories;
using Xunit;

namespace SubscriptionManager.Tests.Services
{
    public class SubscriptionServiceTests
    {
        private readonly Mock<ISubscriptionRepository> _repoMock;
        private readonly SubscriptionService _service;

        public SubscriptionServiceTests()
        {
            _repoMock = new Mock<ISubscriptionRepository>();
            _service = new SubscriptionService(_repoMock.Object);
        }

        [Fact]
        public void GetActiveSubscriptions_ReturnsOnlyActive()
        {
            var data = new List<Subscription>
            {
                new Subscription { Status = "Активна" },
                new Subscription { Status = "Приостановлена" },
                new Subscription { Status = "Активна" }
            };
            _repoMock.Setup(r => r.GetAll()).Returns(data);

            var result = _service.GetActiveSubscriptions().ToList();

            Assert.Equal(2, result.Count);
            Assert.All(result, s => Assert.Equal("Активна", s.Status));
        }

        [Fact]
        public void GetSubscriptionsByCategory_ReturnsOnlyGivenCategory()
        {
            var data = new List<Subscription>
            {
                new Subscription { Category = "Стриминг" },
                new Subscription { Category = "ПО" },
                new Subscription { Category = "Стриминг" }
            };
            _repoMock.Setup(r => r.GetAll()).Returns(data);

            var result = _service.GetSubscriptionsByCategory("Стриминг").ToList();

            Assert.Equal(2, result.Count);
            Assert.All(result, s => Assert.Equal("Стриминг", s.Category));
        }

        [Fact]
        public void AddSubscription_ShouldCallRepoAddAndSave()
        {
            var sub = new Subscription { ServiceName = "New" };

            _service.AddSubscription(sub);

            _repoMock.Verify(r => r.Add(It.Is<Subscription>(x => x == sub)), Times.Once);
            _repoMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateSubscriptionStatus_ShouldUpdateAndSave()
        {
            var sub = new Subscription { Id = 1, Status = "Активна" };
            _repoMock.Setup(r => r.GetById(1)).Returns(sub);

            _service.UpdateSubscriptionStatus(1, "Приостановлена");

            Assert.Equal("Приостановлена", sub.Status);
            _repoMock.Verify(r => r.Update(sub), Times.Once);
            _repoMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateSubscriptionStatus_ShouldThrow_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetById(999)).Returns((Subscription)null);

            Assert.Throws<System.InvalidOperationException>(() => _service.UpdateSubscriptionStatus(999, "Приостановлена"));
        }
    }
}
