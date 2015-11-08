using System;
using System.Runtime.Serialization;
using ServiceStack;

namespace Yaqaap.ServiceInterface.ServiceStack
{
    [DataContract]
    public class AuthUserEntrySession : AuthUserSession
    {
        [DataMember]
        public string PartitionKey { get; set; }

        [DataMember]
        public string RowKey { get; set; }

        public Guid GetUserId()
        {
            return Guid.Parse(PartitionKey);
        }
    }
}