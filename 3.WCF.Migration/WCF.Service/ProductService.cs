using ProductInventory;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WCF.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class ProductService : IProductService, IDisposable
    {
        private readonly ProductInventory.ProductInventory _inventory;
        private IProductServiceCallback _callback;
        private EventHandler<ProductChangeEventArgs> _onProductChangedHandler;

        public ProductService()
        {
            _inventory = ProductInventory.ProductInventory.Instance;
        }

        public Task AddProduct(Product product)
        {
            return _inventory.Create(product);
        }

        public Task<ICollection<Product>> GetProducts()
        {
            return _inventory.GetAll();
        }

        public void Subscribe()
        {
            _callback = OperationContext.Current.GetCallbackChannel<IProductServiceCallback>();
            _inventory.OnProductChanged += OnProductChanged;
        }

        private void OnProductChanged(object sender, ProductChangeEventArgs e)
        {
            try
            {
                _callback.OnUpdate(e.Product);
            }
            catch (CommunicationException)
            {
                // noop
            }
        }

        public void Dispose()
        {
            if (_callback is object)
            {
                _inventory.OnProductChanged -= OnProductChanged;
            }
        }
    }
}
