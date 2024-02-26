using EventStore.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MyEventStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly EventStoreClient client;
        private readonly ILogger<WeatherForecastController> logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
                                          EventStoreClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        [HttpPost]
        public async Task<object> PostAsync(WeatherForecast data)
        {
            var weatherForecastRecordedEvent = new WeatherForecastRecorded
            {
                Date = DateTime.Now,
                Summary = data.Summary,
                TemperatureC = data.TemperatureC
            };

            var utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(weatherForecastRecordedEvent);

            var eventData = new EventData(Uuid.NewUuid(),
                                           nameof(WeatherForecastRecorded),
                                           utf8Bytes.AsMemory());

            var writeResult = await this.client
                            .AppendToStreamAsync(WeatherForecastRecorded.StreamName,
                                                  StreamState.Any,
                                                  new[] { eventData });

            return writeResult;
        }

        [HttpGet]
        public async Task<List<WeatherForecastRecorded>> GetAsync()
        {
            var results = new List<WeatherForecastRecorded>();

            var streamResult = this.client.ReadStreamAsync(Direction.Forwards,
                                                WeatherForecastRecorded.StreamName,
                                                StreamPosition.Start);

            await foreach (var item in streamResult)
            {
                var eventType = item.Event.EventType;

                if (eventType == nameof(WeatherForecastRecorded))
                {
                    var weatherForecastRecordedEvent = JsonSerializer.Deserialize<WeatherForecastRecorded>(item.Event.Data.Span);
                    
                    if(weatherForecastRecordedEvent != null) 
                    {
                        results.Add(weatherForecastRecordedEvent);
                    }              
                }
            }


            return results;
        }
    }
}
