using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    /// <summary>
    /// All users interactions for a target question
    /// </summary>
    public class UserQuestionEntry : TableEntity
    {
        public UserQuestionEntry()
        {
        }

        public UserQuestionEntry(Guid userId, Guid questionId)
        {
            PartitionKey = userId.ToString();
            RowKey = questionId.ToString();
        }

        public DateTime Creation { get; set; }

        public DateTime Modification { get; set; }

        /// <summary>
        /// Listes des guid pour lesques l'utilisateur à voté Up
        /// </summary>
        public string VotesUp { get; set; }  
        
        /// <summary>
        /// Listes des guid pour lesques l'utilisateur à voté Down
        /// </summary>
        public string VotesDown { get; set; }

        /// <summary>
        /// Indique que l'utilisateur aime cette question
        /// </summary>
        public bool Love { get; set; }
    }
}