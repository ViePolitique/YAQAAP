using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    public class LoveEntry : TableEntity
    {
        public LoveEntry()
        {
        }

        public LoveEntry(string targetId, Guid ownerId)
        {
            PartitionKey = targetId;
            RowKey = ownerId.ToString();
        }

        public DateTime Creation { get; set; }

        public DateTime Modification { get; set; }
    }
}