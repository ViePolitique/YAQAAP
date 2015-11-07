using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    public class VoteEntry : TableEntity
    {
        public VoteEntry()
        {
        }

        public VoteEntry(string targetId, Guid ownerId)
        {
            PartitionKey = targetId;
            RowKey = ownerId.ToString();
        }

        public DateTime Creation { get; set; }

        public DateTime Modification { get; set; }

        public int Value { get; set; }
    }
}