using System.Linq;
using ServiceStack;
using Yaqaap.ServiceInterface.Business;
using Yaqaap.ServiceInterface.TableRepositories;
using Yaqaap.ServiceModel;

namespace Yaqaap.ServiceInterface
{
    public class MyServices : Service
    {
        public object Any(Ask request)
        {
            AskResponse response = new AskResponse();
            response.Result = "OK";

            return response;
        }

        public object Any(Search request)
        {
            QuestionEntry[] questions = IndexHelper.Search<QuestionEntry>(request.Question, Tables.Questions);

            // "No similar question found... yet !"
            return new SearchResponse
            {
                Questions = questions.Select(k => new SearchQuestionResponse { Title = k.Title }).ToArray()
            };
        }
    }
}
