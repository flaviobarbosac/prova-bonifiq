using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class RandomService
    {
        private readonly TestDbContext _ctx;
        private readonly Random _random;

        public RandomService(TestDbContext ctx)
        {
            _ctx = ctx;
            _random = new Random(); // Sem semente fixa
        }

        public async Task<int> GetRandom()
        {
            int number;
            bool saved = false;

            do
            {
                number = _random.Next(0, 100);

                // Verifica se já existe no banco
                if (!await _ctx.Numbers.AnyAsync(n => n.Number == number))
                {
                    _ctx.Numbers.Add(new RandomNumber() { Number = number });
                    try
                    {
                        await _ctx.SaveChangesAsync();
                        saved = true;
                    }
                    catch (DbUpdateException)
                    {
                        // Em caso de erro de duplicidade, tenta novamente
                        saved = false;
                    }
                }
            } while (!saved);

            return number;
        }
    }
}