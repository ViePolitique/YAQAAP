using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaqaap.ServiceInterface.ServiceStack;
using Yaqaap.ServiceInterface.TableRepositories;
using Yaqaap.ServiceModel;

namespace Yaqaap.ServiceInterface.Business
{
    public class VoteHelper
    {
        public static VoteKind? GetVoteKindForUser(TableRepository tableRepository, VoteTarget target, Guid questionId, Guid getOwnerId, AuthUserEntrySession userSession)
        {
            if (userSession.IsAuthenticated)
            {
                string votePartitionKey = target + "|" + questionId + "|" + getOwnerId;
                VoteEntry voteEntry = tableRepository.Get<VoteEntry>(Tables.Votes, votePartitionKey, userSession.GetUserId());
                if (voteEntry != null)
                {
                    return voteEntry.Value == 1 ? VoteKind.Up : VoteKind.Down;
                }
            }

            return null;
        }
    }
}
