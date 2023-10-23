using P04WeatherForecastAPI.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P04WeatherForecastAPI.Client.ViewModels
{
    public class DataViewModel
    {
        public DataViewModel(Data data)
        {
            TemperatureYesterday = data.TemperatureYesterday;
            TemperatureSixHoursAgo = data.TemperatureSixHoursAgo;
            TemperatureInOneHour = data.TemperatureInOneHour;
            TemperatureTomorrow = data.TemperatureTomorrow;
            TemperatureInFiveDays = data.TemperatureInFiveDays;
            Alarms = data.Alarms;
    }
        public string TemperatureYesterday { get; set; }
        public string TemperatureSixHoursAgo { get; set; }
        public string TemperatureInOneHour { get; set; }
        public string TemperatureTomorrow { get; set; }
        public string TemperatureInFiveDays { get; set; }
        public string Alarms { get; set; }
    }
}
