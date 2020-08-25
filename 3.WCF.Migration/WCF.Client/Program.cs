using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCF.Client.ProductService;

namespace WCF.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var instanceContext = new InstanceContext(new ProductsCallback());
            var client = new ProductServiceClient(instanceContext);

            await client.AddProductAsync(new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test product",
                Price = 5.99d,
                LastModified = DateTime.UtcNow
            });

            var products = await client.GetProductsAsync();

            foreach (var product in products)
            {
                Console.WriteLine($"{product.Id} - {product.Name} - {product.Price}");
            }

            client.Subscribe();

            while (!Console.KeyAvailable)
            {
                await Task.Delay(300);
            }
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    class ProductsCallback : IProductServiceCallback
    {
        public void OnUpdate(Product product)
        {
            Console.WriteLine($"{product.Id} - {product.Name} - {product.Price}");
        }
    }
}
