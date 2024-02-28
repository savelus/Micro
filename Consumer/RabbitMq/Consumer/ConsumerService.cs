﻿using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Data;
using Rabbit.HttpService;
using Rabbit.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Redis.Redis;

namespace Consumer.RabbitMq.Consumer;

public class ConsumerService : IConsumerService, IDisposable
{
    private const string QueueName = "Links";
    private const string Exchange = "LinkExchange";

    private readonly IModel _model;
    private readonly IConnection _connection;
    private readonly IRedisService _redisService;
    private readonly IHttpService _httpService;

    public ConsumerService(IRabbitMqService rabbitMqService, IRedisService redisService, IHttpService httpService)
    {
        _redisService = redisService;
        _httpService = httpService;
        
        _connection = rabbitMqService.CreateChannel();
        _model = _connection.CreateModel();
        _model.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
        _model.ExchangeDeclare(Exchange, ExchangeType.Fanout, durable: true, autoDelete: false);
        _model.QueueBind(QueueName, Exchange, string.Empty);

        Console.WriteLine("ctor Consumer");
    }

    public async Task ReadMessages()
    {
        Console.WriteLine("ReadMessages");
        var consumer = new AsyncEventingBasicConsumer(_model);
        consumer.Received += async (ch, ea) =>
        {
            var body = ea.Body.ToArray();
            var link = Encoding.UTF8.GetString(body);
            var jsonLink = JsonSerializer.Deserialize<Link>(link);
            Console.WriteLine(jsonLink.Id);
            await RunAsyncGet(jsonLink);
            await Task.CompletedTask;
            _model.BasicAck(ea.DeliveryTag, false);
        };
        _model.BasicConsume(QueueName, false, consumer);
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_model.IsOpen) _model.Close();
        if (_connection.IsOpen) _connection.Close();
    }

    private async Task RunAsyncGet(Link link)
    {
        var statusCode = await _httpService.GetStatusCodeAsync(link.Url);
        
        using var client = new HttpClient();

        client.BaseAddress = new Uri("http://links1:5000");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.Timeout = TimeSpan.FromSeconds(5);
        
        var statusFromCache = await _redisService.Get("redis" + link.Url, statusCode);

        if (statusFromCache != null) {
            link.Status = statusFromCache;
        }
        
        Console.WriteLine($"status from cache = {statusFromCache}");
        Console.WriteLine($"status from http service = {statusCode}");

        link.Status = statusCode;

        var serializeObj = JsonSerializer.Serialize(link);
        var stringContent = new StringContent(serializeObj, Encoding.UTF8, "application/json");

        try {
            var response = await client.PutAsync("/Links/update/", stringContent);
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
}