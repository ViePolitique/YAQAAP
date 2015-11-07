using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    /// <summary>
    /// Only one answer by user,
    /// PartitionKey : questionId
    /// RowKey : ownerId
    /// </summary>
    public class AnswerEntry : TableEntity
    {
        public AnswerEntry()
        {
        }

        public AnswerEntry(Guid questionId, Guid ownerId)
        {
            PartitionKey = questionId.ToString();
            RowKey = ownerId.ToString();
        }

        public Guid GetQuestionId()
        {
            return Guid.Parse(PartitionKey);
        }

        public Guid GetOwnerId()
        {
            return Guid.Parse(RowKey);
        }

        public DateTime Creation { get; set; }
        public DateTime Modification { get; set; }

        public string Content { get; set; }

        public int Votes { get; set; }
    }
}