namespace NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Options
{
    public class ZmqOptions
    {
        public string Host { get; set; } = "localhost";

        public int Port { get; set; } = 5005;

        public int SendHighWatermark { get; set; } = 1000;

        public int RequestTimeout { get; set; } = 1000;
    }
}
