using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;
using System;
using System.Threading.Tasks;

namespace ProvaPub.Services
{
    public interface ITempoProvider
    {
        DateTime UtcNow { get; }
    }

    public class TempoProviderPadrao : ITempoProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }

    public class CustomerService
    {
        TestDbContext _ctx;
        private readonly ITempoProvider _tempoProvider;

        public CustomerService(TestDbContext ctx, ITempoProvider tempoProvider)
        {
            _ctx = ctx;
            _tempoProvider = tempoProvider;
        }

        public CustomerList ListCustomers(int page)
        {
            var (itens, total, proxima) = UtilService.Paginar(_ctx.Customers.OrderBy(x => x.Id), page, 10);
            return new CustomerList() { HasNext = proxima, TotalCount = total, Customers = itens };
        }

        public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
        {
            if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));

            if (purchaseValue <= 0) throw new ArgumentOutOfRangeException(nameof(purchaseValue));

            //Business Rule: Non registered Customers cannot purchase
            var customer = await _ctx.Customers.FindAsync(customerId);
            if (customer == null) throw new InvalidOperationException($"Customer Id {customerId} does not exists");

            //Business Rule: A customer can purchase only a single time per month
            var baseDate = _tempoProvider.UtcNow.AddMonths(-1);
            var ordersInThisMonth = await _ctx.Orders.CountAsync(s => s.CustomerId == customerId && s.OrderDate >= baseDate);
            if (ordersInThisMonth > 0)
                return false;

            //Business Rule: A customer that never bought before can make a first purchase of maximum 100,00
            var haveBoughtBefore = await _ctx.Customers.CountAsync(s => s.Id == customerId && s.Orders.Any());
            if (haveBoughtBefore == 0 && purchaseValue > 100)
                return false;

            //Business Rule: A customer can purchases only during business hours and working days
            var now = _tempoProvider.UtcNow;
            if (now.Hour < 8 || now.Hour > 18 || now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
                return false;


            return true;
        }

    }
}
