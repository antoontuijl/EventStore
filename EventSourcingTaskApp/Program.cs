using EventSourcingTaskApp.Infrastructure;
using EventStore.ClientAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var eventStoreConnection = EventStoreConnection.Create(
                connectionString: builder.Configuration.GetValue<string>("EventStore:ConnectionString"),
                builder: ConnectionSettings.Create().KeepReconnecting(),
                connectionName: builder.Configuration.GetValue<string>("EventStore:ConnectionName"));

eventStoreConnection.ConnectAsync().GetAwaiter().GetResult();

builder.Services.AddSingleton(eventStoreConnection);
builder.Services.AddTransient<AggregateRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
