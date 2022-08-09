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
    public class CityController : ControllerBase
    {
        private readonly WeatherInfoContext _weatherInfoContext;

        public CityController(WeatherInfoContext weatherInfoContext)
        {
            _weatherInfoContext = weatherInfoContext;
        }

        // GET: weatherinfo/<CityController>
        [HttpGet("ListCities")]
        public async Task<IEnumerable<City>> GetCities()
        {
            IEnumerable<City> cities = await _weatherInfoContext.Cities.ToListAsync();
            cities = cities.OrderBy(c => c.Name);
            return cities;
        }

        // POST weatherinfo/<CityController>
        [HttpPost("CreateCity")]
        public async Task<ActionResult<string>> CreateCity(string cityName, string countryName)
        {
            IEnumerable<City> currentCities;

            try
            {
                currentCities = await GetCities();
            }
            catch (Exception e)
            {
                throw new Exception($"Couldn't get cities for comparison. Error: {e}");
            }

            try
            {
                bool doesCityExist = currentCities.Any(cc => cc.Name == cityName);
                if (!doesCityExist)
                {
                    City city = new City();
                    city.Name = cityName;

                    
                    city.CountryId = await GetCountryId(countryName); 

                    if(city.CountryId < 0)
                    {
                        throw new Exception($"The country you entered most likely doesn't exist in the database yet");
                    }

                    await _weatherInfoContext.Cities.AddAsync(city);
                    await _weatherInfoContext.SaveChangesAsync();
                    
                    return CreatedAtRoute(city, city);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error at check for already added cities. Error: {e}");
            }
            return null;
        }

        [HttpDelete("DeleteCities")]
        public async Task<City> DeleteCities()
        {
            IEnumerable<City> cities = await _weatherInfoContext.Cities.ToListAsync();
            _weatherInfoContext.Cities.RemoveRange(cities);
            await _weatherInfoContext.SaveChangesAsync();
            return null;

        }


        private async Task<int> GetCountryId(string name)
        {
            CountryController countryControll = new CountryController(_weatherInfoContext);

            IEnumerable<Country> countries = await countryControll.GetCountries();
            IEnumerable<int> countryId;

            if (countries.Any(c=>c.Name == name))
            {
                countryId = from country in countries
                    where country.Name == name
                    select country.Id;
                return countryId.First();
            }

            return -1;
        }
    }
}
