using Grpc.Core;
using Grpc.Net.Client;
using ProductInventory;
using ProtoBuf.Grpc.Client;
using ProtobufNet.Service.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProtobufNet.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = channel.CreateGrpcService<IProductService>();

            await client.AddProduct(new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test product",
                Price = 5.99d,
                LastModified = DateTime.UtcNow
            });

            var products = await client.GetProducts();

            foreach (var product in products.Items)
            {
                Console.WriteLine($"{product.Id} - {product.Name} - {product.Price}");
            }

            var tokenSource = new CancellationTokenSource();
            var stream = client.Subscribe();

            var task = DisplayUpdates(stream, tokenSource.Token);

            while (!Console.KeyAvailable)
            {
                await Task.Delay(300);
            }

            tokenSource.Cancel();

            await task;
        }

        private static async Task DisplayUpdates(IAsyncEnumerable<Product> stream, CancellationToken token)
        {
            try
            {
                await foreach (var product in stream.WithCancellation(token))
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
