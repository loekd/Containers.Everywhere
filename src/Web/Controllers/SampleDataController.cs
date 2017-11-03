using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Web.Repositories;
using CounterState = Web.Models.CounterState;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ICounterRepository _counterRepository;
        private readonly string _storeType;

        public SampleDataController(ICounterRepository counterRepository, IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _counterRepository = counterRepository ?? throw new ArgumentNullException(nameof(counterRepository));
            _storeType = counterRepository.GetType().Name;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CounterState state)
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

        [HttpGet("[action]")]
        public async Task<IActionResult> PingDb()
        {
            string database = _configuration.GetValue<string>("Database") ?? "db";

            try
            {
                var addresses = await Dns.GetHostAddressesAsync(database).ConfigureAwait(false);
                return Ok(addresses.Select(a => a.ToString()).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest($"Error connecting to database '{database}', exception:{ex.Message}");
            }
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> QueryDb()
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
                string database = _configuration.GetValue<string>("Database");

                return BadRequest($"Error connecting to database '{database}', exception:{ex.Message}");
            }
        }
    }
}
