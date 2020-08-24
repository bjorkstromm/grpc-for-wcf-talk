using System;
using System.Runtime.Serialization;

namespace Sample.Contract
{
    [DataContract]
    public class HelloRequest
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }
    }
}
