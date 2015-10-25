using System;
using System.Net.Configuration;
using ServiceStack;

namespace Yaqaap.ServiceModel
{
    [Route("/answers/{Id}")]
    public class Answers : IReturn<AnswersResponse>
    {
        public string Id { get; set; }
    }
    
    public class AnswersResponse
    {
        public UserCard Creator { get; set; }
        public DateTime Creation { get; set; }
        public long Views { get; set; }

        public string Title { get; set; }
        public string Detail { get; set; }
        public string[] Tags { get; set; }

        public Guid SelectedAnswer { get; set; }

        public Answer [] Answers { get; set; }
    }

    public class Answer
    {
        public UserCard Creator { get; set; }
        public DateTime Creation { get; set; }
        public int Votes { get; set; }

        public string Content { get; set; }
    }

    public class UserCard
    {
        public Guid Id { get; set; }
    }

}