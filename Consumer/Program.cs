using Consumer.RabbitMq.Consumer;
using Consumer.RabbitMq.ConsumerHost;
using Data;
using Rabbit;
using Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddData(builder.Configuration);
builder.Services.AddServices(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddScoped<IConsumerService, ConsumerService>();
builder.Services.AddHostedService<ConsumerHostedService>();

var app = builder.Build();

app.Run();