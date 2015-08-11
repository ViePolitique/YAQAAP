using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    public class QuestionEntry : TableEntity
    {
        public QuestionEntry()
        {
        }

        public QuestionEntry(Guid id)
        {
            PartitionKey = id.ToString();
        }

        public Guid Creator { get; set; }

        public DateTime Creation { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string Tags { get; set; }
    }
}
