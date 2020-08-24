using System;
using System.Runtime.Serialization;

namespace Sample.Contract
{
    [DataContract]
    public class HelloReply
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}
