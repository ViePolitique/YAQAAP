using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    public class QuestionEntry : TableEntity
    {
        public QuestionEntry()
        {
        }

        public QuestionEntry(Guid creatorId, Guid questionId)
        {
            PartitionKey = creatorId.ToString();
            RowKey = questionId.ToString();
        }


        public Guid GetCreatorId()
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
