using Newtonsoft.Json;
using System;
namespace WeatherApp
{
    public class Location
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("localtime")]
        public DateTime LocalTime { get; set; }
    }

    public class Current
    {
        [JsonProperty("temp_c")]
        public double TempC { get; set; }

        [JsonProperty ("temp_f")]
        public double TempF { get; set; }

        [JsonProperty("condition")]
        public Condition Condition { get; set; }

        [JsonProperty("wind_kph")]
        public double WindKph { get; set; }

        [JsonProperty("wind_mph")]
        public double WindMph { get; set; }

        [JsonProperty("humidity")]
        public double Humidity { get; set; }

        [JsonProperty("uv")]
        public double Uv { get; set; }

        [JsonProperty("pressure_mb")]
        public double Pressure_mb { get; set; }

        [JsonProperty("is_day")]
        public bool IsDay {  get; set; }
    }

    public class Condition
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    public class WeatherData
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("current")]
        public Current Current { get; set; }
    }
    public class City
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }
}

