using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using SubscriptionManager.Models;
using SubscriptionManager.Services;
using SubscriptionManager.ViewModels;
using Xunit;

namespace SubscriptionManager.Tests.ViewModels
{
    public class MainWindowViewModelTests
    {
        private MainWindowViewModel CreateVmWithData(IEnumerable<Subscription> items)
        {
            var vm = new MainWindowViewModel();

            vm.Subscriptions.Clear();
            foreach (var s in items)
                vm.Subscriptions.Add(s);

            vm.FilteredSubscriptions.Refresh();
            return vm;
        }

        [Fact]
        public void Filter_ByCategory_Works()
        {
            var data = new[]
            {
                new Subscription { ServiceName="A", Category="Стриминг", Status="Активна", PaymentPeriod="Ежемесячно", Cost=100, NextBillingDate=DateTime.Today },
                new Subscription { ServiceName="B", Category="ПО", Status="Активна", PaymentPeriod="Ежемесячно", Cost=200, NextBillingDate=DateTime.Today }
            };

            var vm = CreateVmWithData(data);
            vm.FilterCategory = "Стриминг";
            vm.FilteredSubscriptions.Refresh();

            var list = vm.FilteredSubscriptions.Cast<Subscription>().ToList();
            Assert.Single(list);
            Assert.Equal("Стриминг", list[0].Category);
        }

        [Fact]
        public void Filter_ByStatus_Works()
        {
            var data = new[]
            {
                new Subscription { ServiceName="A", Category="Стриминг", Status="Активна", PaymentPeriod="Ежемесячно", Cost=100, NextBillingDate=DateTime.Today },
                new Subscription { ServiceName="B", Category="Стриминг", Status="Приостановлена", PaymentPeriod="Ежемесячно", Cost=200, NextBillingDate=DateTime.Today }
            };

            var vm = CreateVmWithData(data);
            vm.FilterStatus = "Активна";
            vm.FilteredSubscriptions.Refresh();

            var list = vm.FilteredSubscriptions.Cast<Subscription>().ToList();
            Assert.Single(list);
            Assert.Equal("Активна", list[0].Status);
        }

        [Fact]
        public void Filter_ByPaymentPeriod_Works()
        {
            var data = new[]
            {
                new Subscription { ServiceName="A", Category="Стриминг", Status="Активна", PaymentPeriod="Ежемесячно", Cost=100, NextBillingDate=DateTime.Today },
                new Subscription { ServiceName="B", Category="Стриминг", Status="Активна", PaymentPeriod="Ежегодно", Cost=1200, NextBillingDate=DateTime.Today }
            };

            var vm = CreateVmWithData(data);
            vm.FilterPaymentPeriod = "Ежегодно";
            vm.FilteredSubscriptions.Refresh();

            var list = vm.FilteredSubscriptions.Cast<Subscription>().ToList();
            Assert.Single(list);
            Assert.Equal("Ежегодно", list[0].PaymentPeriod);
        }

        [Fact]
        public void Summary_MonthlyAndYearly_AfterFiltering()
        {
            var data = new[]
            {
                new Subscription { ServiceName="A", Category="Стриминг", Status="Активна", PaymentPeriod="Ежемесячно", Cost=100, NextBillingDate=DateTime.Today },
                new Subscription { ServiceName="B", Category="Стриминг", Status="Активна", PaymentPeriod="Ежеквартально", Cost=900, NextBillingDate=DateTime.Today }
            };

            var vm = CreateVmWithData(data);

            vm.FilterCategory = "Все";
            vm.FilterStatus = "Все";
            vm.FilterPaymentPeriod = "Все";
            vm.FilteredSubscriptions.Refresh();

            Assert.Equal(400m, Math.Round(vm.MonthlyCost, 2));
            Assert.Equal(4800m, Math.Round(vm.YearlyCost, 2));
        }

        [Fact]
        public void Upcoming_IsDetected_Correctly()
        {
            var data = new[]
            {
                new Subscription { ServiceName="Soon", NextBillingDate = DateTime.Today.AddDays(2), Cost=100, PaymentPeriod="Ежемесячно", Status="Активна", Category="Стриминг" },
                new Subscription { ServiceName="Later", NextBillingDate = DateTime.Today.AddDays(10), Cost=100, PaymentPeriod="Ежемесячно", Status="Активна", Category="Стриминг" }
            };

            var vm = CreateVmWithData(data);

            var upcoming = vm.Subscriptions.Where(s => s.IsUpcoming).ToList();
            Assert.Single(upcoming);
            Assert.Equal("Soon", upcoming[0].ServiceName);
        }

        [Fact]
        public void Commands_CanExecute_EditCommand_False_WhenNoSelection()
        {
            var data = new[]
            {
                new Subscription { ServiceName="A", Category="Стриминг", Status="Активна", PaymentPeriod="Ежемесячно", Cost=100, NextBillingDate=DateTime.Today }
            };

            var vm = CreateVmWithData(data);

            vm.SelectedSubscription = null;
            Assert.False(vm.EditCommand.CanExecute(null));
            Assert.False(vm.SuspendCommand.CanExecute(null));
            Assert.False(vm.CancelCommand.CanExecute(null));
        }

        [Fact]
        public void Commands_CanExecute_EditCommand_True_WhenSelected()
        {
            var data = new[]
            {
                new Subscription { ServiceName="A", Category="Стриминг", Status="Активна", PaymentPeriod="Ежемесячно", Cost=100, NextBillingDate=DateTime.Today }
            };

            var vm = CreateVmWithData(data);
            vm.SelectedSubscription = vm.Subscriptions.First();

            Assert.True(vm.EditCommand.CanExecute(null));
            Assert.True(vm.SuspendCommand.CanExecute(null));
            Assert.True(vm.CancelCommand.CanExecute(null));
        }
    }
}
