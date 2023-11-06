using RabbitMQ.Client;

namespace Event.Bus.Interfaces
{
    public interface IPublishConfiguration
    {
        public bool PublishConfirm { get; set; }
        public bool RetryPublishMessage { get; set; }

        public void ConfigurationChannel(IModel channel)
        {
            if (PublishConfirm)
                channel.ConfirmSelect();

        }
    }
}
