using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    public class IndexEntry : TableEntity
    {
        public IndexEntry()
        {
        }

        public IndexEntry(string id, string term)
        {
            PartitionKey = id;
            RowKey = term;
        }
    }
}