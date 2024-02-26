namespace MyEventStore
{
    public class WeatherForecastRecorded
    {
        public static string StreamName = "WeatherForecast";

        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
    }
}
