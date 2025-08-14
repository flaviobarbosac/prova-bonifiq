using Xunit;
using ProvaPub.Services;
using ProvaPub.Models;
using ProvaPub.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace ProvaPubTest
{
    public class ProductServiceTests
    {
        private TestDbContext GetDbContext(List<Product> products = null)
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new TestDbContext(options);

            if (products != null)
            {
                context.Products.AddRange(products);
                context.SaveChanges();
            }
            return context;
        }

        [Fact]
        public void BuscarProdutosPaginados_Returns_Correct_Page()
        {
            var products = new List<Product>();
            for (int i = 1; i <= 30; i++)
                products.Add(new Product { Id = i, Name = $"Product {i}" });

            var ctx = GetDbContext(products);
            var service = new ProductService(ctx);

            var result = service.BuscarProdutosPaginados(3); // Página 3, deve retornar produtos 21-30

            Assert.Equal(10, result.Products.Count);
            Assert.Equal(30, result.Products[9].Id);
            Assert.False(result.HasNext);
            Assert.Equal(30, result.TotalCount);
        }
    }
}