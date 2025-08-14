using Microsoft.AspNetCore.Mvc;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;

namespace ProvaPub.Controllers
{
	
	/// <summary>
	/// O Código abaixo faz uma chmada para a regra de negócio que valida se um consumidor pode fazer uma compra.
	/// Crie o teste unitário para esse Service. Se necessário, faça as alterações no código para que seja possível realizar os testes.
	/// Tente criar a maior cobertura possível nos testes.
	/// 
	/// Utilize o framework de testes que desejar. 
	/// Crie o teste na pasta "Tests" da solution
	/// </summary>
	[ApiController]
	[Route("[controller]")]
	public class Parte4Controller :  ControllerBase
	{
		private readonly TestDbContext _ctx;
		private readonly ITempoProvider _tempoProvider;

		public Parte4Controller(TestDbContext ctx, ITempoProvider tempoProvider)
		{
			_ctx = ctx;
			_tempoProvider = tempoProvider;
		}

		[HttpGet("CanPurchase")]
		public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
		{
			CustomerService svc = new CustomerService(_ctx, _tempoProvider);
			return await svc.CanPurchase(customerId, purchaseValue);
		}
	}
}
