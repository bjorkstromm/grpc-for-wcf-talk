using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ProductService;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleProtobuf.Service
{
    public class ProductService : global::ProductService.ProductService.ProductServiceBase
    {
        private readonly ILogger<ProductService> _logger;
        private readonly ProductInventory.ProductInventory _inventory;
        private IServerStreamWriter<Product> _subscriberStream;

        public ProductService(ILogger<ProductService> logger, ProductInventory.ProductInventory inventory)
        {
            _logger = logger;
            _inventory = inventory;
        }

        public override async Task<Empty> AddProduct(Product request, ServerCallContext context)
        {
            await _inventory.Create(request.ToDomainModel());

            return new Empty();
        }

        public override async Task<ProductCollection> GetProducts(Empty request, ServerCallContext context)
        {
            var items = await _inventory.GetAll();
            
            return new ProductCollection
            {
                Items = { items.Select(Product.FromDomainModel) }
            };
        }

        public override async Task Subscribe(Empty request, IServerStreamWriter<Product> responseStream, ServerCallContext context)
        {
            _subscriberStream = responseStream;

            _inventory.OnProductChanged += OnProductChanged;
            
            _logger.LogInformation("Subscription started.");
            
            await AwaitCancellation(context.CancellationToken);
            
            _inventory.OnProductChanged -= OnProductChanged;

            _logger.LogInformation("Subscription Finished.");
        }

        private async void OnProductChanged(object sender, ProductInventory.ProductChangeEventArgs e)
        {
            await _subscriberStream.WriteAsync(Product.FromDomainModel(e.Product));
        }

        private static Task AwaitCancellation(CancellationToken token)
        {
            var completion = new TaskCompletionSource<object>();
            token.Register(() => completion.SetResult(null));
            return completion.Task;
        }
    }
}
