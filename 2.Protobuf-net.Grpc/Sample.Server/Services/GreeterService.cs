using System.Threading.Tasks;
using Sample.Contract;
using Microsoft.Extensions.Logging;

namespace Sample.Server
{
    public class GreeterService : IGreeter
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public Task<HelloReply> SayHello(HelloRequest request)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
