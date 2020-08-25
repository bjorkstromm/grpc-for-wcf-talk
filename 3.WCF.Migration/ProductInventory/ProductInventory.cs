using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ProductInventory
{
    public class ProductInventory
    {
        private ConcurrentDictionary<Guid, Product> _inventory = new ConcurrentDictionary<Guid, Product>();

        public event EventHandler<ProductChangeEventArgs> OnProductChanged;

        private static Lazy<ProductInventory> _instance = new Lazy<ProductInventory>(() => new ProductInventory());
        private readonly Timer _timer;

        public static ProductInventory Instance => _instance.Value;

        private ProductInventory()
        {
            _timer = new Timer
            {
                Interval = 5000d,
                AutoReset = true
            };

            _timer.Elapsed += Elapsed;
            _timer.Start();
        }

        private async void Elapsed(object sender, ElapsedEventArgs e)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var name = new string(Enumerable.Repeat(chars, 8)
                  .Select(s => s[random.Next(s.Length)]).ToArray());

            await Create(new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Price = random.Next(1, 10) * 2d,
                LastModified = DateTime.UtcNow
            });
        }

        public Task Create(Product product)
        {
            if (_inventory.TryAdd(product.Id, product))
            {
                OnProductChanged?.Invoke(this, new ProductChangeEventArgs(product));
            }

            return Task.CompletedTask;
        }

        public Task<ICollection<Product>> GetAll()
        {
            return Task.FromResult(_inventory.Values);
        }
    }

    public class ProductChangeEventArgs : EventArgs
    {
        public Product Product { get; }

        public ProductChangeEventArgs(Product product)
        {
            Product = product;
        }
    }
}
