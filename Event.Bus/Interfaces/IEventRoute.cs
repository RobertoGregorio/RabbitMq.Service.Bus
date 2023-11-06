namespace Event.Bus.Interfaces
{
    public interface IEventRoute
    {
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public bool Durable { get; set; }
        public string Queue { get; set; }
        public string Persist { get; set; }
    }
}
