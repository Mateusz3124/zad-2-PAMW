using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using P04WeatherForecastAPI.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace P04WeatherForecastAPI.Client.Services
{
    internal class AccuWeatherService : IAccuWeatherService
    {
        private const string base_url = "http://dataservice.accuweather.com";
        private const string autocomplete_endpoint = "locations/v1/cities/autocomplete?apikey={0}&q={1}&language{2}";
        private const string current_conditions_endpoint = "currentconditions/v1/{0}?apikey={1}&language{2}";
        private const string tomorrow_forecast_endpoint = "forecasts/v1/daily/1day/{0}?apikey={1}&language{2}";
        private const string tomorrow_weather_alarm_endpoint = "alarms/v1/1day/{0}?apikey={1}&language{2}";
        private const string weather_in_five_days = "forecasts/v1/daily/5day/{0}?apikey={1}&language{2}";
        private const string weather_in_one_hour_endpoint = "forecasts/v1/hourly/1hour/{0}?apikey={1}&language{2}";
        private const string weather_yesterday_endpoint = "currentconditions/v1/{0}/historical/24?apikey={1}&language{2}";
        private const string weather_in_past_6_hours_endpoint = "currentconditions/v1/{0}/historical?apikey={1}&language{2}";

        string api_key;
        string language;

        public AccuWeatherService()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<App>()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsetings.json"); 

            var configuration = builder.Build();
            api_key = "KZsVQJV4iFNEE2ijDAVrR0cY7Iayrc9u";
            language = "pl";
        }


        public async Task<City[]> GetLocations(string locationName)
        {
            string uri = base_url + "/" + string.Format(autocomplete_endpoint, api_key, locationName, language);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(uri);
                string json = await response.Content.ReadAsStringAsync();
                City[] cities = JsonConvert.DeserializeObject<City[]>(json);
                return cities;

            }
        }

        public async Task<Weather> GetCurrentConditions(string cityKey)
        {
            string uri = base_url + "/" + string.Format(current_conditions_endpoint, cityKey, api_key,language);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(uri);
                string json = await response.Content.ReadAsStringAsync();
                Weather[] weathers= JsonConvert.DeserializeObject<Weather[]>(json);
                return weathers.FirstOrDefault();
            }
        }

        public async Task<string> DoMultipleTimes(int n, int count,string[] regexTemplates, string uri)
        {
            string infoCurrent = "";
                Regex rx = new Regex(regexTemplates[n]);
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(uri);
                    string json = await response.Content.ReadAsStringAsync();
                    Match match = rx.Match(json);
                    for (int i = 0; i < count-1; i++)
                    {
                        match = match.NextMatch();
                    }
                    infoCurrent = match.Groups[1].Value;
                }
            return infoCurrent;
        }

        public async Task<Data> GetInfo(string cityKey)
        {           
            string[] regexTemplates = new string[6];
            regexTemplates[0] = "\"Temperature\":{\"Value\":(-{0,1}\\d+(\\.\\d+)),\"Unit";
            regexTemplates[1] = "\"Maximum\":{\"Value\":(-{0,1}\\d+(\\.\\d+)),\"Unit";
            regexTemplates[2] = "\"Temperature\":{\"Metric\":{\"Value\":(-{0,1}\\d+(\\.\\d+)),\"Unit\":\"C\",";
            regexTemplates[3] = "\"Maximum\":{\"Value\":(-{0,1}\\d+(\\.\\d+)),\"Unit";
            regexTemplates[5] = "\"Temperature\":{\"Metric\":{\"Value\":(-{0,1}\\d+(\\.\\d+)),\"Unit\":\"C\",";
            string[] result = new string[6];
            string[] UrlForEndpoints = new string[] { weather_in_one_hour_endpoint, tomorrow_forecast_endpoint, weather_yesterday_endpoint, weather_in_five_days, tomorrow_weather_alarm_endpoint, weather_in_past_6_hours_endpoint };
            string uri = "";
            for (int i = 0; i < 2; i++)
            {
                uri = base_url + "/" + string.Format(UrlForEndpoints[i], cityKey, api_key, language);
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(uri);
                    string json = await response.Content.ReadAsStringAsync();
                    Regex rx = new Regex(regexTemplates[i]);
                    Match match = rx.Match(json);
                    string info = match.Groups[1].Value;
                    result[i] = info;
                }
            }
            uri = base_url + "/" + string.Format(UrlForEndpoints[2], cityKey, api_key, language);
            result[3] = await DoMultipleTimes(2, 24, regexTemplates, uri);
            uri = base_url + "/" + string.Format(UrlForEndpoints[3], cityKey, api_key, language);
            result[2] = await DoMultipleTimes(3, 5, regexTemplates, uri);
            uri = base_url + "/" + string.Format(UrlForEndpoints[4], cityKey, api_key, language);
            string infoCurrent = "";
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(uri);
                infoCurrent = await response.Content.ReadAsStringAsync();
            }
            if (infoCurrent != "[]")
            {
                result[5] = "There are weather alarms at your location";
            }
            else
            {
                result[5] = "There are no weather alarms at your location";
            }
            uri = base_url + "/" + string.Format(UrlForEndpoints[5], cityKey, api_key, language);
            result[4] = await DoMultipleTimes(5, 6, regexTemplates, uri);        
            for (int i = 0; i < 3; i++)
            {
                result[i] = result[i].Replace(".", ",");
                double container = double.Parse(result[i]);
                double celcius = (container - 32) * 5 / 9;
                result[i] = celcius.ToString("0.0").Replace(",", ".");
            }
            Data data = new()
            {
                TemperatureYesterday = result[3],
                TemperatureSixHoursAgo = result[4],
                TemperatureInOneHour = result[0],
                TemperatureTomorrow = result[1],
                TemperatureInFiveDays = result[2],
                Alarms = result[5]
            };

            return data;
        }
    }
}
