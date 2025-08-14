using Xunit;
using ProvaPub.Services;
using ProvaPub.Models;
using ProvaPub.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace ProvaPubTest
{
    public class CustomerServicePaginationTests
    {
        private TestDbContext GetDbContext(List<Customer> customers = null)
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
            return context;
        }

        [Fact]
        public void ListCustomers_Returns_Correct_Page()
        {
            var customers = new List<Customer>();
            for (int i = 1; i <= 25; i++)
                customers.Add(new Customer { Id = i, Name = $"Customer {i}" });

            var ctx = GetDbContext(customers);
            var service = new CustomerService(ctx, new TempoProviderPadrao());

            var result = service.ListCustomers(2); // Página 2, deve retornar clientes 11-20

            Assert.Equal(10, result.Customers.Count);
            Assert.Equal(20, result.Customers[9].Id);
            Assert.True(result.HasNext);
            Assert.Equal(25, result.TotalCount);
        }
    }
}