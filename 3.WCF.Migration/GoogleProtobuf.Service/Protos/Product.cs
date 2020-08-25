using Google.Protobuf.WellKnownTypes;
using System;

namespace ProductService
{
    public partial class Product
    {
        public static Product FromDomainModel(ProductInventory.Product product)
        {
            return new Product
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Price = product.Price,
                LastModified = product.LastModified.ToTimestamp()
            };
        }

        public ProductInventory.Product ToDomainModel()
        {
            return new ProductInventory.Product
            {
                Id = Guid.Parse(Id),
                Name = Name,
                Price = Price,
                LastModified = LastModified.ToDateTime()
            };
        }
    }
}
