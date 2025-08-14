using Xunit;
using ProvaPub.Services;
using ProvaPub.Models;
using ProvaPub.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProvaPubTest
{
    public class TempoProviderFake : ITempoProvider
    {
        public DateTime UtcNow { get; set; }
    }

    public class CustomerServiceTests
    {
        private TestDbContext GetDbContext(List<Customer> customers = null, List<Order> orders = null)
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new TestDbContext(options);

            if (customers != null)
            {
                context.Customers.AddRange(customers);
                context.SaveChanges();
            }
            if (orders != null)
            {
                context.Orders.AddRange(orders);
                context.SaveChanges();
            }
            return context;
        }

        [Fact]
        public async Task CustomerId_Must_Be_Greater_Than_Zero()
        {
            var service = new CustomerService(GetDbContext(), new TempoProviderFake { UtcNow = DateTime.UtcNow });
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.CanPurchase(0, 10));
        }

        [Fact]
        public async Task PurchaseValue_Must_Be_Greater_Than_Zero()
        {
            var service = new CustomerService(GetDbContext(), new TempoProviderFake { UtcNow = DateTime.UtcNow });
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.CanPurchase(1, 0));
        }

        [Fact]
        public async Task Customer_Must_Exist()
        {
            var service = new CustomerService(GetDbContext(), new TempoProviderFake { UtcNow = DateTime.UtcNow });
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CanPurchase(999, 10));
        }

        [Fact]
        public async Task Only_One_Purchase_Per_Month()
        {
            var customer = new Customer { Id = 1, Name = "Test" };
            var order = new Order { Id = 1, CustomerId = 1, OrderDate = DateTime.UtcNow };
            var service = new CustomerService(GetDbContext(new List<Customer> { customer }, new List<Order> { order }), new TempoProviderFake { UtcNow = DateTime.UtcNow });
            var result = await service.CanPurchase(1, 10);
            Assert.False(result);
        }

        [Theory]
        [InlineData(DayOfWeek.Saturday)]
        [InlineData(DayOfWeek.Sunday)]
        public async Task Only_Business_Days_False(DayOfWeek day)
        {
            var customer = new Customer { Id = 3, Name = "Test3" };
            var fakeTime = new TempoProviderFake { UtcNow = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc).AddDays((int)day - (int)DayOfWeek.Sunday) };
            var service = new CustomerService(GetDbContext(new List<Customer> { customer }), fakeTime);
            var result = await service.CanPurchase(3, 10);
            Assert.False(result);
        }

        [Theory]
        [InlineData(7)]
        [InlineData(19)]
        public async Task Only_Business_Hours_False(int hour)
        {
            var customer = new Customer { Id = 4, Name = "Test4" };
            var fakeTime = new TempoProviderFake { UtcNow = new DateTime(2023, 1, 2, hour, 0, 0, DateTimeKind.Utc) };
            var service = new CustomerService(GetDbContext(new List<Customer> { customer }), fakeTime);
            var result = await service.CanPurchase(4, 10);
            Assert.False(result);
        }

        [Fact]
        public async Task CanPurchase_True_On_Business_Day_And_Hour()
        {
            var customer = new Customer { Id = 5, Name = "Test5" };
            var fakeTime = new TempoProviderFake { UtcNow = new DateTime(2023, 1, 2, 10, 0, 0, DateTimeKind.Utc) }; // Segunda-feira, 10h
            var service = new CustomerService(GetDbContext(new List<Customer> { customer }), fakeTime);
            var result = await service.CanPurchase(5, 10);
            Assert.True(result);
        }
    }
}
