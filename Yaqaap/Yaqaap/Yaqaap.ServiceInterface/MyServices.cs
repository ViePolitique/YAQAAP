using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;
using Yaqaap.ServiceModel;

namespace Yaqaap.ServiceInterface
{
    public class MyServices : Service
    {
        public object Any(Ask request)
        {
            AskResponse response = new AskResponse();

            return response;
        }

        public object Any(Search request)
        {
            return new SearchResponse
            {
                Questions = new[]
                            {
                                new SearchQuestionResponse() { Title = "Question 1"},
                                new SearchQuestionResponse() { Title = "Question 2"},
                                new SearchQuestionResponse() { Title = "Question 3"},
                                new SearchQuestionResponse() { Title = "Question 4"},
                            }
                // "No similar question found... yet !"
            };
        }
    }
}
