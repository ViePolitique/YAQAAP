using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    /// <summary>
    /// Only one answer by user,
    /// PartitionKey : questionId
    /// RowKey : creatorId
    /// </summary>
    public class AnswerEntry : TableEntity
    {
        public AnswerEntry()
        {
        }

        public AnswerEntry(Guid questionId, Guid creatorId)
        {
            PartitionKey = questionId.ToString();
            RowKey = creatorId.ToString();
        }

        public Guid GetQuestionId()
        {
            return Guid.Parse(PartitionKey);
        }

        public Guid GetCreatorId()
        {
            return Guid.Parse(RowKey);
        }

        public DateTime Creation { get; set; }

        public string Content { get; set; }

        public int Votes { get; set; }
    }
}