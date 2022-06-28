namespace NetCoreCleanArchitecture.Infrastructure.Grpc.Common.Options
{
    public class GrpcOptions
    {
        public string Host { get; set; } = "localhost";

        public int Port { get; set; } = 5005;
    }
}
