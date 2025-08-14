using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class ProductService
    {
        private readonly TestDbContext _db;
        public ProductService(TestDbContext db)
        {
            _db = db;
        }

        public ProductList BuscarProdutosPaginados(int pagina)
        {
            var (itens, total, proxima) = UtilService.Paginar(_db.Products.OrderBy(x => x.Id), pagina, 10);

            return new ProductList
            {
                Products = itens.ToList(),
                TotalCount = total,
                HasNext = proxima
            };
        }
    }
}
