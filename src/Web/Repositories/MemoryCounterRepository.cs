using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Repositories
{
    public class MemoryCounterRepository : ICounterRepository
    {
        private readonly Dictionary<int, CounterState> _counterStates = new Dictionary<int, CounterState>();

        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public Task Add(CounterState state)
        {
            _counterStates.Add(state.Id, state);
            return Task.CompletedTask;
        }

        public Task Update(CounterState state)
        {
            if (_counterStates.TryGetValue(state.Id, out var existingState))
            {
                existingState.Count = state.Count;
                existingState.CreatedAt = state.CreatedAt;
            }
            return Task.CompletedTask;
        }

        public Task Delete(CounterState state)
        {
            if (_counterStates.ContainsKey(state.Id))
            {
                _counterStates.Remove(state.Id);
            }
            return Task.CompletedTask;
        }

        public Task<CounterState> GetLast()
        {
            return Task.FromResult(_counterStates.Values
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefault());
        }
    }
}