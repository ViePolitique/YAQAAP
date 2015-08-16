using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    public class IndexEntry : TableEntity
    {
        public IndexEntry()
        {
        }

        public IndexEntry(string id, string term, string table)
        {
            PartitionKey = id;
            RowKey = $"{table}-{term}";

            RowKey = TableEntityHelper.RemoveDiacritics(RowKey);
            RowKey = TableEntityHelper.ToAzureKeyString(RowKey);
        }
    }
}