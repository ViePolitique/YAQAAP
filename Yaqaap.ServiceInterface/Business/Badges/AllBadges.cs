using System;
using Yaqaap.ServiceInterface.TableRepositories;

namespace Yaqaap.ServiceInterface.Business.Badges
{
    public static class AllBadges
    {
        public static Supporter Supporter => new Supporter();
        public static Critic Critic => new Critic();

        public static Approved Approved => new Approved();
        public static NiceAnswer NiceAnswer => new NiceAnswer();
        public static GoodAnswer GoodAnswer => new GoodAnswer();
        public static GreatAnswer GreatAnswer => new GreatAnswer();


        public static Padawan Padawan => new Padawan();
        public static NiceQuestion NiceQuestion => new NiceQuestion();
        public static GoodQuestion GoodQuestion => new GoodQuestion();
        public static GreatQuestion GreatQuestion => new GreatQuestion();
    }

    /// <summary>
    /// First vote up received on answer
    /// </summary>
    public class Approved : Badge
    {
        public override string Name => "Approved";
        public override BadgeCategory Category => BadgeCategory.AnswerBadges;
    }


    /// <summary>
    /// 10 votes on an answer
    /// </summary>
    public class NiceAnswer : Badge
    {
        public override string Name => "NiceAnswer";
        public override BadgeCategory Category => BadgeCategory.AnswerBadges;
    }

    /// <summary>
    /// 25 votes on an answer
    /// </summary>
    public class GoodAnswer : Badge
    {
        public override string Name => "GoodAnswer";
        public override BadgeCategory Category => BadgeCategory.AnswerBadges;
    }

    /// <summary>
    /// 100 votes on an anwser
    /// </summary>
    public class GreatAnswer : Badge
    {
        public override string Name => "GreatAnswer";
        public override BadgeCategory Category => BadgeCategory.AnswerBadges;
    }





    /// <summary>
    /// First vote up received on question
    /// </summary>
    public class Padawan : Badge
    {
        public override string Name => "Padawan";
        public override BadgeCategory Category => BadgeCategory.QuestionBadges;
    }


    /// <summary>
    /// 10 votes on an question
    /// </summary>
    public class NiceQuestion : Badge
    {
        public override string Name => "NiceQuestion";
        public override BadgeCategory Category => BadgeCategory.QuestionBadges;
    }

    /// <summary>
    /// 25 votes on an question
    /// </summary>
    public class GoodQuestion : Badge
    {
        public override string Name => "GoodQuestion";
        public override BadgeCategory Category => BadgeCategory.QuestionBadges;
    }

    /// <summary>
    /// 100 votes on an question
    /// </summary>
    public class GreatQuestion : Badge
    {
        public override string Name => "GreatQuestion";
        public override BadgeCategory Category => BadgeCategory.QuestionBadges;
    }






    /// <summary>
    /// First vote up
    /// </summary>
    public class Supporter : Badge
    {
        public override string Name => "Supporter";
        public override BadgeCategory Category => BadgeCategory.ModerationBadges;
    }

    /// <summary>
    /// First vote down
    /// </summary>
    public class Critic : Badge
    {
        public override string Name => "Critic";
        public override BadgeCategory Category => BadgeCategory.ModerationBadges;
    }

}