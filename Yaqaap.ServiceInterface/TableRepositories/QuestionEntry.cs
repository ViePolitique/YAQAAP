﻿using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    /// <summary>
    /// PartitionKey : UserId
    /// RowKey : QuestionId
    /// </summary>
    public class QuestionEntry : TableEntity
    {
        public QuestionEntry()
        {
        }

        public QuestionEntry(Guid userId, Guid questionId)
        {
            PartitionKey = userId.ToString();
            RowKey = questionId.ToString();
        }


        public Guid GetUserId()
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

        public long Views { get; set; }

        /// <summary>
        /// UserId that posted the answer
        /// </summary>
        public Guid SelectedAnswer { get; set; }
    }
}
