using ServiceStack;

namespace Yaqaap.ServiceModel
{
    [Route("/search/{Question}")]
    public class Search : IReturn<SearchResponse>
    {
        public string Question { get; set; }
    }

    public class SearchResponse
    {
        public SearchQuestionResponse[] Questions { get; set; }
    }

    public class SearchQuestionResponse
    {
        public string Title { get; set; }
    }
}