using Microsoft.AspNetCore.Mvc;
using ProvaPub.Models;
using ProvaPub.Services;
using ProvaPub.Repository;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProvaPub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Parte3Controller : ControllerBase
    {
        private readonly OrderService _servicoPedido;

        public Parte3Controller(IEnumerable<IPagamentoTipo> metodosPagamento, TestDbContext contexto)
        {
            _servicoPedido = new OrderService(metodosPagamento, contexto);
        }

        [HttpGet("orders")]
        public async Task<Order> PlaceOrder(string metodoPagamento, decimal valorPagamento, int idCliente)
        {
            var pedido = await _servicoPedido.PayOrder(metodoPagamento, valorPagamento, idCliente);
            // Converte OrderDate para UTC-3 antes de retornar
            pedido.OrderDate = pedido.OrderDate.AddHours(-3);
            return pedido;
        }
    }
}
