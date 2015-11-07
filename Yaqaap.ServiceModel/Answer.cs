using System;
using ServiceStack;

namespace Yaqaap.ServiceModel
{
    [Authenticate]
    [Route("/answer")]
    public class Answer : IReturn<AnswerResponse>
    {
        public Guid QuestionOwnerId { get; set; }
        public Guid QuestionId { get; set; }
        public string Content { get; set; }
    }

    public class AnswerResponse
    {
        public ErrorCode Result { get; set; }
    }
}