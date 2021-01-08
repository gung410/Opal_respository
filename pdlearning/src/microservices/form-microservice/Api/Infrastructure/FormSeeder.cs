using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microservice.Form.Common.Helpers;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Domain.ValueObjects.Questions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Form.Infrastructure
{
    public class FormSeeder : IDbContextSeeder
    {
        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
            /*
            var formId = new Guid("91bd6a35-39bd-4698-b8c1-e799b0ce5405");
            var existingForm = context.Forms.IgnoreQueryFilters().FirstOrDefault(p => p.Id == formId);
            if (existingForm != null)
            {
                return;
            }

            var formCreatorId = new Guid("69eff5da-e335-47be-abd1-38916c2ece12");
            var formAnswerId = Guid.NewGuid();
            var questionId1 = Guid.NewGuid();
            var questionId2 = Guid.NewGuid();
            var questionId3 = Guid.NewGuid();
            var questionId4 = Guid.NewGuid();
            var questionId5 = Guid.NewGuid();
            var questionId6 = Guid.NewGuid();

            context.Forms.Add(new FormEntity
            {
                Type = FormType.Quiz,
                Id = formId,
                Title = "Sample Quizzes Form",
                Status = FormStatus.Published,
                CreatedBy = formCreatorId,
                OwnerId = formCreatorId,
                CreatedDate = Clock.Now.AddDays(-1),
                IsDeleted = false,
                InSecondTimeLimit = null,
                MaxAttempt = (short?)100,
                RandomizedQuestions = true
            });

            context.FormQuestions.AddRange(new List<FormQuestion>
            {
                new FormQuestion
                {
                    Id = questionId1, FormId = formId, CreatedDate = Clock.Now.AddDays(-1), CreatedBy = formCreatorId, RandomizedOptions = false, Score = 1.0, Priority = 0,
                    Question_Type = QuestionType.ShortText, Question_Title = "Short Text Question", Question_CorrectAnswer = "CorrectAnswer", Question_Hint = "Short Text Question Hint",
                    Question_FeedbackOnCorrectAnswers = "Correct Answer is: CorrectAnswer"
                },
                new FormQuestion
                {
                    Id = questionId2, FormId = formId, CreatedDate = Clock.Now.AddDays(-1), CreatedBy = formCreatorId, RandomizedOptions = false, Score = 1.0, Priority = 1,
                    Question_Type = QuestionType.LongText, Question_Title = "Long Text Question", Question_Hint = "Long Text Question Hint",
                },
                new FormQuestion
                {
                    Id = questionId3, FormId = formId, CreatedDate = Clock.Now.AddDays(-1), CreatedBy = formCreatorId, RandomizedOptions = true, Score = 1.0,  Priority = 2,
                    Question_Type = QuestionType.SingleChoice,
                    Question_Title = "Single Choice Question",
                    Question_CorrectAnswer = "A",
                    Question_Hint = "Single Choice Question Hint",
                    Question_Options = F.List<QuestionOption>(new QuestionOption { Code = 1, Value = "A" }, new QuestionOption { Code = 2, Value = "B" }),
                    Question_FeedbackOnCorrectAnswers = "Correct Answer is: A"
                },
                new FormQuestion
                {
                    Id = questionId4, FormId = formId, CreatedDate = Clock.Now.AddDays(-1), CreatedBy = formCreatorId, RandomizedOptions = true, Score = 2.0, Priority = 3,
                    Question_Type = QuestionType.MultipleChoice,
                    Question_Title = "Multiple Choice Question",
                    Question_CorrectAnswer = F.List("A", "B"),
                    Question_Hint = "Multiple Choice Question Hint",
                    Question_Options = F.List<QuestionOption>(new QuestionOption { Code = 1, Value = "A" }, new QuestionOption { Code = 2, Value = "B" }, new QuestionOption { Code = 3, Value = "C" }),
                    Question_FeedbackOnCorrectAnswers = "Correct Answer is: A;B"
                },
                new FormQuestion
                {
                    Id = questionId5, FormId = formId, CreatedDate = Clock.Now.AddDays(-1), CreatedBy = formCreatorId, RandomizedOptions = true, Score = 1.0, Priority = 4,
                    Question_Type = QuestionType.TrueFalse,
                    Question_Title = "True/False Choice Question",
                    Question_CorrectAnswer = false,
                    Question_Hint = "True/False Question Hint",
                    Question_Options = F.List<QuestionOption>(new QuestionOption { Code = 1, Value = true }, new QuestionOption { Code = 2, Value = false }),
                    Question_FeedbackOnCorrectAnswers = "Correct Answer is: False"
                }
            });

            context.FormAnswers.Add(new FormAnswer
            {
                Id = formAnswerId,
                Attempt = 1,
                CreatedDate = Clock.Now,
                FormId = formId,
                FormMetaData = new FormAnswerFormMetaData
                {
                    QuestionIdOrderList = F.List(questionId3, questionId1, questionId2, questionId5, questionId6, questionId4),
                    FormQuestionOptionsOrderInfoList = F.List(
                        new FormQuestionOptionsOrderInfo { FormQuestionId = questionId3, OptionCodeOrderList = F.List(2, 1) },
                        new FormQuestionOptionsOrderInfo { FormQuestionId = questionId4, OptionCodeOrderList = F.List(2, 1, 3) },
                        new FormQuestionOptionsOrderInfo { FormQuestionId = questionId5, OptionCodeOrderList = F.List(2, 1) })
                },
                OwnerId = formCreatorId,
                CreatedBy = formCreatorId,
                SubmitDate = null,
                StartDate = Clock.Now,
                EndDate = null,
                Score = 0,
                ScorePercentage = 0
            });

            context.FormQuestionAnswers.AddRange(
                new FormQuestionAnswer { Id = Guid.NewGuid(), FormAnswerId = formAnswerId, FormQuestionId = questionId1, AnswerValue = "WrongAnswer", Score = 0, MaxScore = 1.0, CreatedDate = Clock.Now, CreatedBy = formCreatorId },
                new FormQuestionAnswer { Id = Guid.NewGuid(), FormAnswerId = formAnswerId, FormQuestionId = questionId2, AnswerValue = "My long long answer", Score = 0.5, MaxScore = 1.0, ScoredBy = formCreatorId, CreatedDate = Clock.Now, CreatedBy = formCreatorId },
                new FormQuestionAnswer { Id = Guid.NewGuid(), FormAnswerId = formAnswerId, FormQuestionId = questionId3, AnswerValue = "A", Score = 1.0, MaxScore = 1.0, CreatedDate = Clock.Now, CreatedBy = formCreatorId },
                new FormQuestionAnswer { Id = Guid.NewGuid(), FormAnswerId = formAnswerId, FormQuestionId = questionId4, AnswerValue = F.List("A", "C"), Score = 0.5, MaxScore = 1.0, CreatedDate = Clock.Now, CreatedBy = formCreatorId },
                new FormQuestionAnswer { Id = Guid.NewGuid(), FormAnswerId = formAnswerId, FormQuestionId = questionId5, AnswerValue = true, Score = 0, MaxScore = 1.0, CreatedDate = Clock.Now, CreatedBy = formCreatorId });

             */
        }
    }
}
