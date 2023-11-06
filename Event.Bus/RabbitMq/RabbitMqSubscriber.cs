using Event.Bus.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Event.Bus.RabbitMq
{
    public class RabbitMqSubscriber : IEventBus, ISubscriber
    {
        private static IConnection connection;
        private EventingBasicConsumer consumer;

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

        public void Consume(IBaseEventHandler baseEventHandler, IEventRoute evenRoute, IConsumeConfiguration? configuration = null)
        {
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: evenRoute.Exchange, type: ExchangeType.Direct, durable: evenRoute.Durable, autoDelete: false, arguments: null);

            channel.QueueDeclare(queue: evenRoute.Queue, durable: evenRoute.Durable, autoDelete: false, arguments: null);

            channel.QueueBind(queue: evenRoute.Queue, exchange: evenRoute.Exchange, routingKey: evenRoute.RoutingKey, arguments: null);

            channel.BasicQos(1, 1, false);

            consumer = new EventingBasicConsumer(channel);

            consumer.Registered += Consumer_Registered;

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    await baseEventHandler.Handle(message);

                    channel.BasicAck(ea.DeliveryTag, false);
                    Console.Write("message has consume payload: " + message);
                }
                catch (Exception ex)
                {
                    channel.BasicNack(ea.DeliveryTag, false, true);
                    Console.Write(ex.Message);
                }
            };

            channel.BasicConsume(queue: evenRoute.Queue,
                                 autoAck: false,
                                 consumer: consumer);

        }

        private void Consumer_Registered(object? sender, ConsumerEventArgs e)
        {
            Console.Write($"Consumer {e.ConsumerTags} has been registered");
        }

        private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            throw new Exception(e.ReplyText);
        }
    }
}
