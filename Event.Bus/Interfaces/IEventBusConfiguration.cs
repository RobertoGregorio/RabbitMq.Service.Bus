namespace Event.Bus.Interfaces
{
    public interface IEventBusConfiguration
    {
        public string Hostname { get; set; }
        public string VirtualHost { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string Port { get; set; }
        public bool AutomaticRecouvery { get; set; }
        public int ConnectionTimeoutMilliseconds { get; set; }
        public int NetworkRecoveryInterval { get; set; }
    }
}
