
using EventSourcingTaskApp.Core.Framework;
using EventStore.ClientAPI;
using System.Text;
using System.Text.Json;

namespace EventSourcingTaskApp.Infrastructure;
public class AggregateRepository
{
    private readonly IEventStoreConnection _eventStore;
    private readonly ILogger<AggregateRepository> _logger;

    public AggregateRepository(IEventStoreConnection eventStore,
         ILogger<AggregateRepository> logger)
    {
        _eventStore = eventStore;
        _logger = logger;
    }

    public async Task SaveAsync<T>(T aggregate) where T : Aggregate, new()
    {
        var events = aggregate.GetChanges()
            .Select(@event => new EventStore.ClientAPI.EventData(
                Guid.NewGuid(),
                @event.GetType().Name,
                true,
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event)),
                Encoding.UTF8.GetBytes(@event.GetType().FullName)))
            .ToArray();

        if (!events.Any())
        {
            return;
        }

        var streamName = GetStreamName(aggregate, aggregate.Id);

        var result = await _eventStore.AppendToStreamAsync(streamName, ExpectedVersion.Any, events);
    }

    public async Task<T> LoadAsync<T>(Guid aggregateId) where T : Aggregate, new()
    {
        if (aggregateId == Guid.Empty)
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(aggregateId));

        var aggregate = new T();
        var streamName = GetStreamName(aggregate, aggregateId);

        try
        {
            var nextPageStart = 0L;

            do
            {
                var page = await _eventStore.ReadStreamEventsForwardAsync(
                    streamName, nextPageStart, 4096, false);

                if (page.Events.Length > 0)
                {
                    aggregate.Load(
                        page.Events.Last().Event.EventNumber,
                        page.Events.Select(@event => JsonSerializer.Deserialize(Encoding.UTF8.GetString(@event.OriginalEvent.Data), Type.GetType(Encoding.UTF8.GetString(@event.OriginalEvent.Metadata)))
                        ).ToArray());
                }

                nextPageStart = !page.IsEndOfStream ? page.NextEventNumber : -1;
            } while (nextPageStart != -1);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
       

        return aggregate;
    }

    private string GetStreamName<T>(T type, Guid aggregateId) => $"{type.GetType().Name}-{aggregateId}";
}
