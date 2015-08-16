using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    public class QuestionEntry : TableEntity
    {
        public QuestionEntry()
        {
        }

        public QuestionEntry(Guid id, Guid creator)
        {
            PartitionKey = creator.ToString();
            RowKey = id.ToString();
        }


        public Guid GetCreator()
        {
            return Guid.Parse(PartitionKey);
        }

        public Guid GetId()
        {
            return Guid.Parse(RowKey);
        }

        public DateTime Creation { get; set; }

        public string Title { get; set; }

        public string Detail { get; set; }

        public string Tags { get; set; }
    }
}
