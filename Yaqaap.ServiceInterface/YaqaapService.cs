using System;
using System.Linq;
using System.Net;
using Microsoft.WindowsAzure.Storage.Table;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Support.Markdown;
using Yaqaap.ServiceInterface.Business;
using Yaqaap.ServiceInterface.TableRepositories;
using Yaqaap.ServiceModel;

namespace Yaqaap.ServiceInterface
{
    public class YaqaapService : Service
    {
        public object Any(Ask request)
        {
            AskResponse response = new AskResponse();
            response.Result = ErrorCode.OK;

            Guid creatorId = GetUserId();

            QuestionEntry questionEntry = new QuestionEntry(creatorId, Guid.NewGuid())
            {
                Title = request.Title,
                Detail = request.Detail,
                Creation = DateTime.UtcNow,
                Tags = string.Join(",", request.Tags).ToLowerInvariant()
            };

            TableRepository tableRepository = new TableRepository();
            tableRepository.InsertOrReplace(questionEntry, Tables.Questions);

            IndexHelper.CreateIndex(questionEntry.GetId(), request.Title + " " + questionEntry.Tags, Tables.Questions);

            return response;
        }

        public object Any(Search request)
        {
            QuestionEntry[] questions = IndexHelper.Search<QuestionEntry>(request.Question, Tables.Questions);

            // "No similar question found... yet !"
            return new SearchResponse
            {
                Questions = questions.Select(k => new SearchQuestionResponse { Id = k.GetId(), Title = k.Title }).ToArray()
            };
        }

        public object Any(Answers request)
        {
            TableRepository tableRepository = new TableRepository();
            CloudTable questionTable = tableRepository.GetTable(Tables.Questions);

            Guid questionId = Guid.Parse(request.Id);

            TableQuery<QuestionEntry> questionQuery = questionTable.CreateQuery<QuestionEntry>();

            QuestionEntry questionEntry = (from k in questionQuery
                                           where k.RowKey == questionId.ToString()
                                           select k).SingleOrDefault();

            if (questionEntry == null)
                throw HttpError.NotFound("Such question do not exist");

            questionEntry.Views++;
            tableRepository.InsertOrMerge(questionEntry, Tables.Questions);

            CloudTable answerTable = tableRepository.GetTable(Tables.Answers);
            TableQuery<AnswerEntry> answerQuery = answerTable.CreateQuery<AnswerEntry>();

            AnswerEntry[] answerEntries = (from k in answerQuery
                                           where k.PartitionKey == questionId.ToString()
                                           select k).ToArray();

            AnswersResponse answersResponse = new AnswersResponse
            {
                Creation = questionEntry.Creation,
                Creator = CreateUserCard(questionEntry.GetUserId()),
                Detail = questionEntry.Detail,
                Tags = questionEntry.Tags.SplitAndTrimOn(new char[] { ',' }),
                Title = questionEntry.Title,
                Views = questionEntry.Views,
                SelectedAnswer = questionEntry.SelectedAnswer,
                Answers = answerEntries.Select(k => new AnswerResult
                {
                    Creator = CreateUserCard(k.GetCreatorId()),
                    Creation = k.Creation,
                    Content = k.Content,
                    Votes = k.Votes
                }).ToArray()
            };

            return answersResponse;
        }


        public object Any(Answer request)
        {
            Guid creatorId = GetUserId();

            TableRepository tableRepository = new TableRepository();
            AnswerEntry answerQuery = new AnswerEntry(request.QuestionId, creatorId)
            {
                Content = request.Content,
                Creation = DateTime.UtcNow,
                Votes = 0
            };

            tableRepository.InsertOrReplace(answerQuery, Tables.Answers);

            AnswerResponse response = new AnswerResponse
            {
                Result = ErrorCode.OK
            };


            return response;
        }

        private Guid GetUserId()
        {
            //IAuthSession session = GetSession();
            //return Guid.Parse(session.UserAuthId);
            return Guid.Empty; // todo
        }

        private UserCard CreateUserCard(Guid creatorId)
        {
            return new UserCard
            {
                Id = creatorId
            };
        }
    }
}
