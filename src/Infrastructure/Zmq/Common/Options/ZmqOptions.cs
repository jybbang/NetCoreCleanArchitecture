namespace NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Options
{
    public class ZmqOptions
    {
        public int Port { get; set; } = 5005;

        public int SendHighWatermark { get; set; } = 1000;
    }
}
