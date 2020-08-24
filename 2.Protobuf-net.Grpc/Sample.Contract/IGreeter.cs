using System.ServiceModel;
using System.Threading.Tasks;

namespace Sample.Contract
{
    [ServiceContract]
    public interface IGreeter
    {
        [OperationContract]
        Task<HelloReply> SayHello(HelloRequest request);
    }
}
