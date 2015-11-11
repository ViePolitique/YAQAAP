using System;
using ServiceStack;

namespace Yaqaap.ServiceModel
{
    public enum VoteKind
    {
        None = 0,
        Up = 1,
        Down = -1
    }

    public enum VoteTarget
    {
        Question,
        Answer
    }

    [Authenticate]
    [Route("/vote")]
    public class Vote : IReturn<VoteResponse>
    {
        public Guid QuestionId { get; set; }

        /// <summary>
        /// If VoteTarget if Question => it's questionOwnerId
        /// If VoteTarget if Answer => it's answerOwnerId
        /// </summary>
        public Guid OwnerId { get; set; }

        public VoteKind VoteKind { get; set; }
        public VoteTarget VoteTarget { get; set; }
    }

    public class VoteResponse
    {
        public ErrorCode Result { get; set; }
        public int VoteValue { get; set; }
        public VoteKind VoteKind { get; set; }
    }
}