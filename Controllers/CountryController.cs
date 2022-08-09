using LancomWeatherInfoAPI.Data;
using LanconWeatherAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LancomWeatherInfoAPI.Controllers
{
    [Route("weatherinfo/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly WeatherInfoContext _weatherInfoContext;

        public CountryController(WeatherInfoContext weatherInfoContext)
        {
            _weatherInfoContext = weatherInfoContext;
        }

        // GET: weatherinfo/<ValuesController>
        [HttpGet(Name = "GetCountries")]
        public async Task<IEnumerable<Country>> GetCountries()
        {
            var countries = _weatherInfoContext.Countries.Include(c => c.Cities).AsNoTracking();
            return await countries.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Country>> CreateCountries([FromBody] IEnumerable<Country> countries)
        {
            IEnumerable<Country> currentCountries;

            IList<Country> validCountries = new List<Country>();
            try
            {
                currentCountries = await GetCountries();
            }
            catch (Exception e)
            {
                throw new Exception($"Couldn't get Countries for comparison. Error: {e}");
            }

            try
            {
                foreach (var country in countries)
                {
                    if (currentCountries.Any(cc => cc.Name == country.Name))
                    {
                        continue;
                    }

                    validCountries.Add(country);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error at check for already added countries. Error: {e}");
            }

            try
            {
                await _weatherInfoContext.Countries.AddRangeAsync(validCountries);
                await _weatherInfoContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Error at saving countries to database. Error: {e}");
            }

            return CreatedAtRoute(validCountries, validCountries);
        }
    }
}
