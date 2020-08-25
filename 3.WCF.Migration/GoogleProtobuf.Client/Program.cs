using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using ProductService;
using System;
using System.Threading;
using System.Threading.Tasks;
using ProductServiceClient = ProductService.ProductService.ProductServiceClient;

namespace GoogleProtobuf.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new ProductServiceClient(channel);

            await client.AddProductAsync(new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test product",
                Price = 5.99d,
                LastModified = DateTime.UtcNow.ToTimestamp()
            });

            var products = await client.GetProductsAsync(new Empty());

            foreach (var product in products.Items)
            {
                Console.WriteLine($"{product.Id} - {product.Name} - {product.Price}");
            }

            var tokenSource = new CancellationTokenSource();
            var subcription = client.Subscribe(new Empty());

            var task = DisplayUpdates(subcription.ResponseStream, tokenSource.Token);

            while (!Console.KeyAvailable)
            {
                await Task.Delay(300);
            }

            tokenSource.Cancel();

            await task;
        }

        private static async Task DisplayUpdates(IAsyncStreamReader<Product> stream, CancellationToken token)
        {
            try
            {
                await foreach (var product in stream.ReadAllAsync(token))
                {
                    Console.WriteLine($"{product.Id} - {product.Name} - {product.Price}");
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Cancelled)
                {
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Finished.");
            }
        }
    }
}
