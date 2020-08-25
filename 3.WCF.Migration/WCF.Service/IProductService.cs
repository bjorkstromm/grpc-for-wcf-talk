using ProductInventory;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WCF.Service
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IProductServiceCallback))]
    public interface IProductService
    {
        [OperationContract]
        Task AddProduct(Product product);

        [OperationContract]
        Task<ICollection<Product>> GetProducts();

        [OperationContract(IsOneWay = true)]
        void Subscribe();
    }

    [ServiceContract]
    public interface IProductServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnUpdate(Product product);
    }
}
