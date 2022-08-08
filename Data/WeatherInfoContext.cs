using LanconWeatherAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LancomWeatherInfoAPI.Data
{
    public class WeatherInfoContext:DbContext
    {
        public WeatherInfoContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
    }
}
