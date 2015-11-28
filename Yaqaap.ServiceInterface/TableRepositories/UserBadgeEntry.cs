using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    public class UserBadgeEntry : TableEntity
    {
        public UserBadgeEntry()
        {
        }

        public UserBadgeEntry(Guid userId, string badgeName)
        {
            PartitionKey = userId.ToString();
            RowKey = badgeName;
            Awarded = DateTime.UtcNow;
        }

        /// <summary>
        /// When this badge was earned
        /// </summary>
        public DateTime Awarded { get; set; }

        public Guid? QuestionId { get; set; }
    }
}