using System;
using ServiceStack;

namespace Yaqaap.ServiceModel
{
    [Route("/answer/{Id}/{Title}")]
    public class Answer : IReturn<AnswerResponse>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
    
    public class AnswerResponse
    {
        public string Result { get; set; }
    }
}