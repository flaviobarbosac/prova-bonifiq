namespace ProvaPub.Services
{
    public class UtilService
    {
        public static (List<T> itens, int total, bool proxima) Paginar<T>(IQueryable<T> query, int pagina, int tamanho)
        {
            int total = query.Count();
            var itens = query.Skip((pagina - 1) * tamanho).Take(tamanho).ToList();
            bool proxima = (pagina * tamanho) < total;
            return (itens, total, proxima);
        }
    }
}
