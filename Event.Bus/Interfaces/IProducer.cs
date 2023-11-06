namespace Event.Bus.Interfaces
{
    internal interface IProducer
    {
        public void Publish(IEventRoute evenRoute, object payload);

        public void Publish(IEventRoute evenRoute, object payload, IPublishConfiguration? publishConfiguration = null);
    }
}
