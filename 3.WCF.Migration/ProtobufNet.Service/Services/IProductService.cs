using ProductInventory;
using ProtoBuf.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ProtobufNet.Service.Services
{
    [DataContract]
    public class ProductCollection
    {
        [DataMember(Order = 1)]
        public ICollection<Product> Items { get; set; } = new List<Product>();
    }

    [ServiceContract]
    public interface IProductService
    {
        [OperationContract]
        ValueTask AddProduct(Product product);

        [OperationContract]
        ValueTask<ProductCollection> GetProducts();

        [OperationContract]
        IAsyncEnumerable<Product> Subscribe(CallContext context = default);
    }
}
