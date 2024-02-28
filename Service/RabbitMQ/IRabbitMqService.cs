using RabbitMQ.Client;

namespace Rabbit.RabbitMQ;

public interface IRabbitMqService
{
    void SendMessage(object obj);
    IConnection CreateChannel();
}