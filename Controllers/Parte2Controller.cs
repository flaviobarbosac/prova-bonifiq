using Microsoft.AspNetCore.Mvc;
using ProvaPub.Models;
using ProvaPub.Services;

namespace ProvaPub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Parte2Controller : ControllerBase
    {
        private readonly ProductService _produtoServico;
        private readonly CustomerService _clienteServico;

        public Parte2Controller(ProductService produtoServico, CustomerService clienteServico)
        {
            _produtoServico = produtoServico;
            _clienteServico = clienteServico;
        }

        [HttpGet("products")]
        public ProductList ListarProdutos(int pagina)
        {
            return _produtoServico.BuscarProdutosPaginados(pagina);
        }

        [HttpGet("customers")]
        public CustomerList ListarClientes(int pagina)
        {
            return _clienteServico.ListCustomers(pagina);
        }
    }
}