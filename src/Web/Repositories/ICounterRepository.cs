using System.Threading.Tasks;

namespace Web.Repositories
{
    public interface ICounterRepository
    {
        Task Initialize();

        Task Add(CounterState state);

        Task Update(CounterState state);

        Task Delete(CounterState state);

        Task<CounterState> GetLast();
    }
}