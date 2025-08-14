using Xunit;
using ProvaPub.Services;
using ProvaPub.Models;
using ProvaPub.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ProvaPubTest
{
    public class PagadorFake : IPagamentoTipo
    {
        public string NomeMetodo => "fake";
        public Task<Order> PagarAsync(Order pedido, decimal valor, int clienteId, TestDbContext contexto)
        {
            pedido.Value = valor;
            pedido.CustomerId = clienteId;
            pedido.OrderDate = DateTime.UtcNow;
            return Task.FromResult(pedido);
        }
    }

    public class OrderServiceTests
    {
        private TestDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new TestDbContext(options);
        }

        [Fact]
        public async Task PayOrder_Uses_Correct_Payment_Method()
        {
            var pagamentos = new List<IPagamentoTipo> { new PagadorFake() };
            var ctx = GetDbContext();
            var service = new OrderService(pagamentos, ctx);

            var order = await service.PayOrder("fake", 123.45m, 99);

            Assert.Equal(123.45m, order.Value);
            Assert.Equal(99, order.CustomerId);
            Assert.True(order.OrderDate <= DateTime.UtcNow);
        }

        [Fact]
        public async Task PayOrder_Throws_If_Method_Not_Found()
        {
            var pagamentos = new List<IPagamentoTipo> { new PagadorFake() };
            var ctx = GetDbContext();
            var service = new OrderService(pagamentos, ctx);

            await Assert.ThrowsAsync<Exception>(() => service.PayOrder("pix", 10, 1));
        }
    }
}