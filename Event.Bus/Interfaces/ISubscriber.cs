namespace Event.Bus.Interfaces
{
    internal interface ISubscriber
    {
        public void Consume(IBaseEventHandler baseEventHandler, IEventRoute evenRoute, IConsumeConfiguration? configuration = null);
    }
}
