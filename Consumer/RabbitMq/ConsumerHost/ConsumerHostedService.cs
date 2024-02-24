﻿using Consumer.RabbitMq.Consumer;

namespace Consumer.RabbitMq.ConsumerHost;

public class ConsumerHostedService : BackgroundService
{
    private readonly IConsumerService _consumerService;

    public ConsumerHostedService(IConsumerService consumerService)
    {
        _consumerService = consumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("run");
        await _consumerService.ReadMessages();
    }
}