using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    /// <summary>
    /// Only one answer by user,
    /// PartitionKey : questionId
    /// RowKey : userId
    /// </summary>
    public class AnswerEntry : TableEntity
    {
        public AnswerEntry()
        {
        }

        public AnswerEntry(Guid questionId, Guid userId)
        {
            PartitionKey = userId.ToString();
            RowKey = questionId.ToString();
        }

        public Guid GetQuestionId()
        {
            return Guid.Parse(PartitionKey);
        }

        public Guid GetUserId()
        {
            return Guid.Parse(RowKey);
        }

        public DateTime Creation { get; set; }

        public string Content { get; set; }

        public int Votes { get; set; }
    }
}