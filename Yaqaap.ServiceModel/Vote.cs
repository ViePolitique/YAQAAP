using System;
using ServiceStack;

namespace Yaqaap.ServiceModel
{
    public enum VoteKind
    {
        Up,
        Down
    }

    public enum VoteTarget
    {
        Question,
        Answer
    }

    [Route("/vote")]
    public class Vote : IReturn<VoteResponse>
    {
        public Guid QuestionId { get; set; }

        public Guid AnswerOwnerId { get; set; }

        public Guid QuestionOwnerId { get; set; }

        public VoteKind VoteKind { get; set; }
        public VoteTarget VoteTarget { get; set; }
    }

    public class VoteResponse
    {
        public ErrorCode Result { get; set; }
    }
}