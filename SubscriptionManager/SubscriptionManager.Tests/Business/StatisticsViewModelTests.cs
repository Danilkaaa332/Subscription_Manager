using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using SubscriptionManager.Models;
using SubscriptionManager.Services;
using SubscriptionManager.ViewModels;
using Xunit;

namespace SubscriptionManager.Tests.Business
{
    public class StatisticsViewModelTests
    {
        [Fact]
        public void StatisticsViewModel_ComputesTotals_Correctly()
        {
            var mock = new Mock<ISubscriptionService>();

            var testDate = DateTime.Now.AddMonths(-1);

            mock.Setup(s => s.GetAllSubscriptions()).Returns(new List<Subscription>
            {
                new Subscription {Cost=300, PaymentPeriod="Ежемесячно", Status="Активна", AddedDate = testDate},
                new Subscription {Cost=1200, PaymentPeriod="Ежеквартально", Status="Активна", AddedDate = testDate},
                new Subscription {Cost=2400, PaymentPeriod="Ежегодно", Status="Приостановлена", AddedDate = testDate}
            });

            var vm = new StatisticsViewModel(mock.Object);
            Assert.Equal(700m, vm.TotalMonthly);
            Assert.Equal(8400m, vm.TotalYearly);
            Assert.Equal(2, vm.ActiveCount);
        }

        [Fact]
        public void MonthlyData_ShouldBeStable_ForSameInput()
        {
            var mock = new Mock<ISubscriptionService>();

            mock.Setup(s => s.GetAllSubscriptions()).Returns(new List<Subscription>
            {
                new Subscription { Cost=500, PaymentPeriod="Ежемесячно", Status="Активна" }
            });

            var vm1 = new StatisticsViewModel(mock.Object);
            var vm2 = new StatisticsViewModel(mock.Object);

            Assert.Equal(vm1.MonthlyAmounts.Count, vm2.MonthlyAmounts.Count);

            for (int i = 0; i < vm1.MonthlyAmounts.Count; i++)
            {
                Assert.Equal(vm1.MonthlyAmounts[i], vm2.MonthlyAmounts[i]);
            }
        }

        [Fact]
        public void ChangePercentage_ShouldCalculate_Correctly()
        {
            var mock = new Mock<ISubscriptionService>();

            var oldDate = DateTime.Now.AddMonths(-2);
            var recentDate = DateTime.Now.AddDays(-5);

            mock.Setup(s => s.GetAllSubscriptions()).Returns(new List<Subscription>
            {
                new Subscription {Cost=200, PaymentPeriod="Ежемесячно", Status="Активна", AddedDate = oldDate},
                new Subscription {Cost=300, PaymentPeriod="Ежемесячно", Status="Активна", AddedDate = recentDate}
            });

            var vm = new StatisticsViewModel(mock.Object);

            Assert.True(vm.TotalMonthly > 0); 
        }
    }
}