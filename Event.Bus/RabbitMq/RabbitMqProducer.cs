using Event.Bus.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Event.Bus.RabbitMq
{
    internal class RabbitMqProducer : IEventBus, IProducer
    {
        private static IConnection connection;


        public void Setup(IEventBusConfiguration eventBusConfiguration)
        {
            if (connection == null)
            {
                connection = new ConnectionFactory()
                {
                    HostName = eventBusConfiguration.Hostname,
                    Password = eventBusConfiguration.Password,
                    AutomaticRecoveryEnabled = eventBusConfiguration.AutomaticRecouvery,
                    RequestedConnectionTimeout = TimeSpan.FromMilliseconds(eventBusConfiguration.ConnectionTimeoutMilliseconds),
                    NetworkRecoveryInterval = TimeSpan.FromMilliseconds(eventBusConfiguration.NetworkRecoveryInterval),
                    VirtualHost = eventBusConfiguration.VirtualHost
                }.CreateConnection();

                connection.ConnectionShutdown += Connection_ConnectionShutdown;
            }
        }

        public void Publish(IEventRoute evenRoute, object payload, IPublishConfiguration? publishConfiguration)
        {
            using var channel = connection.CreateModel();


            if (publishConfiguration != null)
            {
                if (publishConfiguration.PublishConfirm)
                {
                    channel.ConfirmSelect();
                    channel.BasicNacks += Channel_BasicNacks;
                }

            }

            channel.ExchangeDeclare(exchange: evenRoute.Exchange, type: ExchangeType.Direct, durable: evenRoute.Durable, autoDelete: false, arguments: null);

            channel.QueueDeclare(queue: evenRoute.Queue, durable: evenRoute.Durable, autoDelete: false, arguments: null);

            channel.QueueBind(queue: evenRoute.Queue, exchange: evenRoute.Exchange, routingKey: evenRoute.RoutingKey, arguments: null);

            var properties = channel.CreateBasicProperties();

            var payloadSerializer = JsonSerializer.Serialize(payload);
            var bytes = Encoding.UTF8.GetBytes(payloadSerializer);

            channel.BasicPublish(evenRoute.Exchange, evenRoute.RoutingKey, properties, bytes);
        }

        private void Channel_BasicNacks(object? sender, RabbitMQ.Client.Events.BasicNackEventArgs e)
        {
            throw new Exception(e.DeliveryTag.ToString());
        }

        private void Channel_BasicAcks(object? sender, RabbitMQ.Client.Events.BasicNackEventArgs e)
        {

            Console.Write($"Publish tag: {e.DeliveryTag} has been Ack");
        }

        private IConnection CreateConnection(IEventBusConfiguration eventBus)
        {
            throw new NotImplementedException();
        }

        private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            throw new Exception(e.ReplyText);
        }

        public void Publish(IEventRoute evenRoute, object payload)
        {
            throw new NotImplementedException();
        }
    }
}
