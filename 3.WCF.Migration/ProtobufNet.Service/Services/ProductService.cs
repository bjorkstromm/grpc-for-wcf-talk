using Microsoft.Extensions.Logging;
using ProductInventory;
using ProtoBuf.Grpc;
using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ProtobufNet.Service.Services
{
    public class ProductService : IProductService, IDisposable
    {
        private readonly ILogger<ProductService> _logger;
        private readonly ProductInventory.ProductInventory _inventory;
        private ChannelWriter<Product> _subscriberStream;

        public ProductService(ILogger<ProductService> logger, ProductInventory.ProductInventory inventory)
        {
            _logger = logger;
            _inventory = inventory;
        }

        public async ValueTask AddProduct(Product product)
        {
            await _inventory.Create(product);
        }

        public async ValueTask<ProductCollection> GetProducts()
        {
            return new ProductCollection
            {
                Items = await _inventory.GetAll()
            };
        }

        public IAsyncEnumerable<Product> Subscribe(CallContext context = default)
        {
            var buffer = Channel.CreateUnbounded<Product>();
            _subscriberStream = buffer.Writer;

            _inventory.OnProductChanged += OnProductChanged;

            return buffer.AsAsyncEnumerable(context.CancellationToken);
        }

        public void Dispose()
        {
            if (_subscriberStream is object)
            {
                _inventory.OnProductChanged -= OnProductChanged;
            }
        }

        private async void OnProductChanged(object sender, ProductChangeEventArgs e)
        {
            await _subscriberStream.WriteAsync(e.Product);
        }
    }
}
