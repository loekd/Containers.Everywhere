using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Repositories;
using CounterState = Web.Models.CounterState;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly ICounterRepository _counterRepository;

        public SampleDataController(ICounterRepository counterRepository)
        {
            _counterRepository = counterRepository ?? throw new ArgumentNullException(nameof(counterRepository));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _counterRepository.Initialize();

            var state = await _counterRepository
                .Query()
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (state == null)
                return NotFound();

            return Ok(state);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CounterState state)
        {
            await _counterRepository.Initialize();

            bool exists = false;
            var existingState = await _counterRepository
                .Query()
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (existingState == null)
            {
                existingState = new Repositories.CounterState();
            }
            else
            {
                exists = true;
            }

            existingState.CreatedAt = DateTimeOffset.UtcNow;
            
            if (!state.Count.HasValue || Math.Abs(state.Count.Value - existingState.Count) > 10)
            {
                existingState.Count += 1;
            }
            else
            {
                existingState.Count = state.Count.Value;
            }

            if (exists)
            {
                await _counterRepository.Update(existingState);
            }
            else
            {
                await _counterRepository.Add(existingState);
            }

            return Ok(existingState);
        }
    }
}
