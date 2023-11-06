using RabbitMQ.Client;

namespace Event.Bus.Interfaces
{
    public interface IEventBus
    {
        public void Setup(IEventBusConfiguration eventBus);
    }
}
