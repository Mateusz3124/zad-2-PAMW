using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P04WeatherForecastAPI.Client.Models
{
    public class Data
    {
        public string TemperatureYesterday { get; set; }
        public string TemperatureSixHoursAgo { get; set; }
        public string TemperatureInOneHour { get; set; }
        public string TemperatureTomorrow { get; set; }
        public string TemperatureInFiveDays { get; set; }
        public string Alarms { get; set; }
    }
}