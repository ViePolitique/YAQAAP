using System;
using ServiceStack;

namespace Yaqaap.ServiceModel
{
    [Route("/top")]
    public class Top : IReturn<TopResponse>
    {
        
    }

    public class TopResponse
    {
        public TopQuestionResponse[] Questions { get; set; }
    }

    public class TopQuestionResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Votes { get; set; }
        public int Answers { get; set; }
        public long Views { get; set; }
    }
}