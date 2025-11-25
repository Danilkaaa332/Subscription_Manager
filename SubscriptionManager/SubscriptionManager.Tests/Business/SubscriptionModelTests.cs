using System;
using Xunit;
using SubscriptionManager.Models;

namespace SubscriptionManager.Tests.Business
{
    public class SubscriptionModelTests
    {
        [Theory]
        [InlineData(300, "Ежемесячно", 300)]
        [InlineData(900, "Ежеквартально", 300)]
        [InlineData(3600, "Ежегодно", 300)]
        public void MonthlyCost_ShouldBeCalculatedCorrectly(decimal cost, string period, decimal expected)
        {
            var s = new Subscription { Cost = cost, PaymentPeriod = period };
            Assert.Equal(expected, s.MonthlyCost);
        }

        [Fact]
        public void MonthlyCost_Rounding_ShouldWork()
        {
            var s = new Subscription { Cost = 1000m, PaymentPeriod = "Ежегодно" };
            Assert.Equal(83.33m, s.MonthlyCost);
        }

        [Theory]
        [InlineData(300, "Ежемесячно", 3600)]
        [InlineData(900, "Ежеквартально", 3600)]
        [InlineData(3600, "Ежегодно", 3600)]
        public void YearlyCost_ShouldBeCalculatedCorrectly(decimal cost, string period, decimal expected)
        {
            var s = new Subscription { Cost = cost, PaymentPeriod = period };
            Assert.Equal(expected, s.YearlyCost);
        }

        [Fact]
        public void IsUpcoming_ShouldBeTrue_WhenWithin3Days()
        {
            var s = new Subscription { NextBillingDate = DateTime.Now.Date.AddDays(2) };
            Assert.True(s.IsUpcoming);
        }

        [Fact]
        public void IsUpcoming_ShouldBeFalse_WhenMoreThan3Days()
        {
            var s = new Subscription { NextBillingDate = DateTime.Now.Date.AddDays(5) };
            Assert.False(s.IsUpcoming);
        }

        [Fact]
        public void Clone_ShouldCreateSeparateCopy()
        {
            var s = new Subscription
            {
                ServiceName = "Test",
                Category = "ПО",
                Cost = 123,
                PaymentPeriod = "Ежемесячно",
                NextBillingDate = DateTime.Today,
                ManagementUrl = "http://",
                Notes = "n",
                Status = "Активна"
            };

            var clone = s.Clone();

            Assert.NotSame(s, clone);
            Assert.Equal(s.ServiceName, clone.ServiceName);
            Assert.Equal(s.Cost, clone.Cost);
            Assert.Equal(s.PaymentPeriod, clone.PaymentPeriod);
        }

        [Fact]
        public void CopyFrom_ShouldOverwriteFields()
        {
            var original = new Subscription
            {
                ServiceName = "A",
                Cost = 100,
                PaymentPeriod = "Ежемесячно"
            };

            var source = new Subscription
            {
                ServiceName = "B",
                Cost = 200,
                PaymentPeriod = "Ежегодно"
            };

            original.CopyFrom(source);

            Assert.Equal("B", original.ServiceName);
            Assert.Equal(200, original.Cost);
            Assert.Equal("Ежегодно", original.PaymentPeriod);
        }

        [Fact]
        public void CopyFrom_ShouldNotThrow_WhenSourceIsNull()
        {
            var original = new Subscription
            {
                ServiceName = "A",
                Cost = 100
            };

            var exception = Record.Exception(() => original.CopyFrom(null));
            Assert.Null(exception);
        }
        [Fact]
        public void Cost_ShouldThrowException_WhenNegative()
        {
            var subscription = new Subscription();
            var exception = Assert.Throws<ArgumentException>(() =>subscription.Cost = -100m);
            Assert.Contains("Стоимость не может быть отрицательной", exception.Message);
        }

        [Fact]
        public void Cost_ShouldAcceptZero()
        {
            var subscription = new Subscription();
            subscription.Cost = 0m;
            Assert.Equal(0m, subscription.Cost);
        }

        [Fact]
        public void Cost_ShouldAcceptPositiveValue()
        {
            var subscription = new Subscription();
            subscription.Cost = 150m;
            Assert.Equal(150m, subscription.Cost);
        }
    }
}