using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Repositories;
using CounterState = Web.Models.CounterState;
using Microsoft.AspNetCore.Hosting;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly ICounterRepository _counterRepository;
        private readonly string _storeType;

        public SampleDataController(ICounterRepository counterRepository, IHostingEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _counterRepository = counterRepository ?? throw new ArgumentNullException(nameof(counterRepository));
            _storeType = counterRepository.GetType().Name;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                await _counterRepository.Initialize();

                var state = await _counterRepository
                    .GetLast();

                if (state == null)
                    return NotFound();

                return Ok(new CounterState
                {
                    Count = state.Count,
                    CreatedAt = state.CreatedAt,
                    Store = _storeType
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CounterState state)
        {
            try
            {
                await _counterRepository.Initialize();

                bool exists = false;
                var existingState = await _counterRepository
                    .GetLast();

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

                return Ok(new CounterState
                {
                    Count = existingState.Count,
                    CreatedAt = existingState.CreatedAt,
                    Store = _storeType
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> PingDb([FromQuery]string dbHostName = "db")
        {
            try
            {
                var addresses = await Dns.GetHostAddressesAsync(dbHostName).ConfigureAwait(false);
                return Ok(addresses.Select(a => a.ToString()).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[action]")]
        public IActionResult Environment()
        {
            return Ok(_env.EnvironmentName);
        }       
    }
}
