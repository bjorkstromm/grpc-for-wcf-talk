using System;
using System.Runtime.Serialization;

namespace ProductInventory
{
    [DataContract]
    public class Product
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public double Price { get; set; }

        [DataMember(Order = 4)]
        public DateTime LastModified { get; set; }
    }
}
