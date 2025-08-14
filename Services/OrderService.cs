using ProvaPub.Models;
using ProvaPub.Repository;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ProvaPub.Services
{
    public interface IPagamentoTipo
    {
        Task<Order> PagarAsync(Order pedido, decimal valor, int clienteId, TestDbContext contexto);
        string NomeMetodo { get; }
    }
                
    public class PagadorPix : IPagamentoTipo
    {
        public string NomeMetodo => "pix";
        public async Task<Order> PagarAsync(Order pedido, decimal valor, int clienteId, TestDbContext contexto)
        {
            // lógica do pagamento via Pix
            pedido.Value = valor;
            pedido.CustomerId = clienteId;
            pedido.OrderDate = System.DateTime.UtcNow;
            var order = (await contexto.Orders.AddAsync(pedido)).Entity;
            await contexto.SaveChangesAsync();
            return order;
        }
    }

    public class PagadorCartao : IPagamentoTipo
    {
        public string NomeMetodo => "creditcard";
        public async Task<Order> PagarAsync(Order pedido, decimal valor, int clienteId, TestDbContext contexto)
        {
            // lógica do pagamento via Cartão
            pedido.Value = valor;
            pedido.CustomerId = clienteId;
            pedido.OrderDate = System.DateTime.UtcNow;
            var order = (await contexto.Orders.AddAsync(pedido)).Entity;
            await contexto.SaveChangesAsync();
            return order;
        }
    }

    public class PagadorPaypal : IPagamentoTipo
    {
        public string NomeMetodo => "paypal";
        public async Task<Order> PagarAsync(Order pedido, decimal valor, int clienteId, TestDbContext contexto)
        {
            // lógica do pagamento via Paypal
            pedido.Value = valor;
            pedido.CustomerId = clienteId;
            pedido.OrderDate = System.DateTime.UtcNow;
            var order = (await contexto.Orders.AddAsync(pedido)).Entity;
            await contexto.SaveChangesAsync();
            return order;
        }
    }

    public class OrderService
    {
        private readonly IEnumerable<IPagamentoTipo> _pagamentos;
        private readonly TestDbContext _ctx;

        public OrderService(IEnumerable<IPagamentoTipo> pagamentos, TestDbContext ctx)
        {
            _pagamentos = pagamentos;
            _ctx = ctx;
        }

        public async Task<Order> PayOrder(string metodoPagamento, decimal valorPagamento, int idCliente)
        {
            var metodo = _pagamentos.FirstOrDefault(x => 
    string.Equals(x.NomeMetodo, metodoPagamento, StringComparison.OrdinalIgnoreCase));
            if (metodo == null) throw new System.Exception("Método de pagamento não encontrado");
            var pedido = new Order();
            var order = await metodo.PagarAsync(pedido, valorPagamento, idCliente, _ctx);
            return order;
        }
    }
}
