using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    public class UserEntry : TableEntity
    {
        public UserEntry()
        {
        }

        public UserEntry(Guid id, string username)
        {
            PartitionKey = id.ToString();
            RowKey = username;

            RowKey = TableEntityHelper.RemoveDiacritics(RowKey);
            RowKey = TableEntityHelper.ToAzureKeyString(RowKey);
        }

        public string Password { get; set; }
    }
}