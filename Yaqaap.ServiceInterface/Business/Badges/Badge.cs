using System;
using Yaqaap.ServiceInterface.TableRepositories;

namespace Yaqaap.ServiceInterface.Business.Badges
{
    public abstract class Badge
    {
        public abstract string Name { get; }
        public abstract BadgeCategory Category { get; }

        public void CreateIfNotExist(TableRepository tableRepository, Guid userId)
        {
            UserBadgeEntry badge = tableRepository.Get<UserBadgeEntry>(Tables.UserBadges, userId, Name);
            if (badge == null)
            {
                badge = new UserBadgeEntry(userId, Name);
                tableRepository.InsertOrMerge(badge, Tables.UserBadges);
            }
        }

        public void CreateForQuestion(TableRepository tableRepository, Guid userId, Guid questionId)
        {
            UserBadgeEntry badge = new UserBadgeEntry(userId, Name)
                                   {
                                        QuestionId = questionId,
                                    };
            tableRepository.InsertOrMerge(badge, Tables.UserBadges);
        }
    }
}
