﻿using P04WeatherForecastAPI.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P04WeatherForecastAPI.Client.ViewModels
{
    public class WeatherViewModel
    {
        public WeatherViewModel(Weather weather)
        {
            CurrentTemperature = weather.Temperature.Metric.Value.ToString();
        }
        public String CurrentTemperature { get; set; }
    }
}
