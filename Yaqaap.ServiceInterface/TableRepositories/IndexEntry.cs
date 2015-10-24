using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    public class IndexEntry : TableEntity
    {
        public IndexEntry()
        {
        }

        public IndexEntry(Guid id, string term, string table)
        {
            PartitionKey = id.ToString();
            RowKey = $"{table}-{term}";

            RowKey = TableEntityHelper.RemoveDiacritics(RowKey);
            RowKey = TableEntityHelper.ToAzureKeyString(RowKey);
        }
    }
}