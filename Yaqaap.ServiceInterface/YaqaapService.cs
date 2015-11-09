using System;
using System.Linq;
using System.Net;
using Microsoft.WindowsAzure.Storage.Table;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Support.Markdown;
using Yaqaap.ServiceInterface.Business;
using Yaqaap.ServiceInterface.ServiceStack;
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

            Guid creatorId = UserSession.GetUserId();

            DateTime dateTime = DateTime.UtcNow;
            QuestionEntry questionEntry = new QuestionEntry(creatorId, Guid.NewGuid())
            {
                Title = request.Title,
                Detail = request.Detail,
                Creation = dateTime,
                Modification = dateTime,
                Tags = string.Join(",", request.Tags).ToLowerInvariant()
            };

            TableRepository tableRepository = new TableRepository();
            tableRepository.InsertOrReplace(questionEntry, Tables.Questions);

            IndexHelper.CreateIndex(questionEntry.GetId(), request.Title + " " + questionEntry.Tags, Tables.Questions);

            return response;
        }

        public object Any(Top request)
        {
            TableRepository tableRepository = new TableRepository();
            var questions = tableRepository.GetTable(Tables.Questions).CreateQuery<QuestionEntry>().ToArray().Take(10);

            // "No similar question found... yet !"
            return new TopResponse
            {
                Questions = questions.Select(k => new TopQuestionResponse
                {
                    Id = k.GetId(),
                    Title = k.Title,
                    Votes = k.Votes,
                    Answers = k.Answers,
                    Views = k.Views
                }).ToArray()
            };
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
                Id = questionEntry.GetId(),
                Creation = questionEntry.Creation,
                Owner = CreateUserCard(tableRepository, questionEntry.GetOwnerId()),
                Detail = questionEntry.Detail,
                Tags = questionEntry.Tags.SplitAndTrimOn(new char[] { ',' }),
                Title = questionEntry.Title,
                Views = questionEntry.Views,
                Votes = questionEntry.Votes,
                SelectedAnswer = questionEntry.SelectedAnswer,
                Answers = answerEntries.Select(k => new AnswerResult
                {
                    Owner = CreateUserCard(tableRepository, k.GetOwnerId()),
                    Creation = k.Creation,
                    Content = k.Content,
                    Votes = k.Votes
                }).ToArray()
            };

            // quest user vote for this question
            if (UserSession.IsAuthenticated)
            {
                string votePartitionKey = VoteTarget.Question + "|" + questionId + "|" + questionEntry.GetOwnerId();
                VoteEntry voteEntry = tableRepository.Get<VoteEntry>(Tables.Votes, votePartitionKey, UserSession.GetUserId());
                if (voteEntry != null)
                {
                    answersResponse.VoteKind = voteEntry.Value == 1 ? VoteKind.Up : VoteKind.Down;
                }
            }

            return answersResponse;
        }


        public object Any(Answer request)
        {
            Guid userId = UserSession.GetUserId();

            TableRepository tableRepository = new TableRepository();

            // create answers
            AnswerEntry answerEntry = tableRepository.Get<AnswerEntry>(Tables.Answers, request.QuestionId, userId);
            if (answerEntry == null)
            {
                DateTime dateTime = DateTime.UtcNow;
                answerEntry = new AnswerEntry(request.QuestionId, userId)
                {
                    Content = request.Content,
                    Creation = dateTime,
                    Modification = dateTime,
                    Votes = 0
                };

                // update the answers count
                QuestionEntry questionEntry = tableRepository.Get<QuestionEntry>(Tables.Questions, request.QuestionOwnerId, request.QuestionId);
                if (questionEntry != null)
                {
                    questionEntry.Answers++;
                    tableRepository.InsertOrMerge(questionEntry, Tables.Questions);
                }
            }
            else
            {
                // perform an edit
                answerEntry.Content = request.Content;
                answerEntry.Modification = DateTime.UtcNow;
            }

            tableRepository.InsertOrMerge(answerEntry, Tables.Answers);

            AnswerResponse response = new AnswerResponse
            {
                Result = ErrorCode.OK
            };


            return response;
        }

        public object Any(Vote request)
        {
            Guid userId = UserSession.GetUserId();


            VoteResponse response = new VoteResponse
            {
                Result = ErrorCode.OK,
            };

            TableRepository tableRepository = new TableRepository();

            bool isNew = false;
            string votePartitionKey = request.VoteTarget + "|" + request.QuestionId + "|" + request.OwnerId;
            VoteEntry voteEntry = tableRepository.Get<VoteEntry>(Tables.Votes, votePartitionKey, userId);

            // Création d'un nouveau vote
            if (voteEntry == null)
            {
                isNew = true;
                DateTime dateTime = DateTime.UtcNow;
                voteEntry = new VoteEntry(votePartitionKey, userId)
                {
                    Creation = dateTime,
                    Modification = dateTime,
                    Value = request.VoteKind == VoteKind.Up ? 1 : -1
                };
            }
            else
            {
                // s'il existe un vote existant et que sa valeur n'as pas changé
                if (voteEntry.Value == (request.VoteKind == VoteKind.Up ? 1 : -1))
                {
                    response.Result = ErrorCode.Aborted;
                    return response;
                }

                voteEntry.Modification = DateTime.UtcNow;
            }


            if (request.VoteTarget == VoteTarget.Answer)
            {
                AnswerEntry answerEntry = tableRepository.Get<AnswerEntry>(Tables.Answers, request.QuestionId, request.OwnerId);
                if (!isNew)
                    answerEntry.Votes -= voteEntry.Value;

                voteEntry.Value = (request.VoteKind == VoteKind.Up ? 1 : -1);
                answerEntry.Votes += voteEntry.Value;
                tableRepository.InsertOrMerge(answerEntry, Tables.Answers);

                response.VoteValue = answerEntry.Votes;
            }
            else
            {
                QuestionEntry questionEntry = tableRepository.Get<QuestionEntry>(Tables.Questions, request.OwnerId, request.QuestionId);
                if (!isNew)
                    questionEntry.Votes -= voteEntry.Value;

                voteEntry.Value = (request.VoteKind == VoteKind.Up ? 1 : -1);
                questionEntry.Votes += voteEntry.Value;
                tableRepository.InsertOrMerge(questionEntry, Tables.Questions);

                response.VoteValue = questionEntry.Votes;
            }

            // insert le vote
            tableRepository.InsertOrReplace(voteEntry, Tables.Votes);



            return response;
        }

        private AuthUserEntrySession UserSession => SessionAs<AuthUserEntrySession>();

        private UserCard CreateUserCard(TableRepository tableRepository, Guid creatorId)
        {
            UserEntry user = tableRepository.Get<UserEntry>(Tables.Users, creatorId.ToString())?.FirstOrDefault();

            return new UserCard
            {
                Id = creatorId,
                Username = user?.UserName
            };
        }
    }
}
