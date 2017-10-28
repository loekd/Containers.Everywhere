using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Web.Repositories
{
    public class CounterRepository : ICounterRepository
    {
        private readonly CounterStateDbContext _dbContext;

        public CounterRepository(CounterStateDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task Initialize()
        {
            return _dbContext.Database.EnsureCreatedAsync();
        }

        public Task Add(CounterState state)
        {
            _dbContext.CounterStates.Add(state);
            return _dbContext.SaveChangesAsync();
        }

        public Task Update(CounterState state)
        {
            _dbContext.CounterStates.Update(state);
            return _dbContext.SaveChangesAsync();
        }

        public Task Delete(CounterState state)
        {
            _dbContext.CounterStates.Remove(state);
            return _dbContext.SaveChangesAsync();
        }

        public Task<CounterState> GetLast()
        {
            return _dbContext.CounterStates
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
