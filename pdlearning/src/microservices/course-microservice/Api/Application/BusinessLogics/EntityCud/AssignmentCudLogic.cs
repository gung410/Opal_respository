using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics.EntityCud
{
    public class AssignmentCudLogic : BaseEntityCudLogic<Assignment>
    {
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly IRepository<QuizAssignmentForm> _quizAssignmentFormRepository;
        private readonly IRepository<QuizAssignmentFormQuestion> _quizAssignmentFormQuestionRepository;
        private readonly GetAggregatedContentSharedQuery _aggregatedContentSharedQuery;

        public AssignmentCudLogic(
            IWriteOnlyRepository<Assignment> repository,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IRepository<QuizAssignmentForm> quizAssignmentFormRepository,
            IRepository<QuizAssignmentFormQuestion> quizAssignmentFormQuestionRepository,
            GetAggregatedContentSharedQuery aggregatedContentSharedQuery,
            IUserContext userContext,
            IThunderCqrs thunderCqrs) : base(repository, thunderCqrs, userContext)
        {
            _readAssignmentRepository = readAssignmentRepository;
            _quizAssignmentFormRepository = quizAssignmentFormRepository;
            _quizAssignmentFormQuestionRepository = quizAssignmentFormQuestionRepository;
            _aggregatedContentSharedQuery = aggregatedContentSharedQuery;
        }

        public async Task Insert(Assignment entity, CreateOrUpdateAssignmentCommandQuizForm quizForm, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            var quizAssignmentFormModel = await CreateQuizAssignmentForm(entity, quizForm);

            await SendAssignmentChangeEvent(entity, AssignmentChangeType.Created, quizAssignmentFormModel, cancellationToken);
        }

        public async Task InsertMany(
            List<Assignment> entities,
            List<QuizAssignmentForm> quizAssignmentForms,
            List<QuizAssignmentFormQuestion> quizAssignmentFormQuestions,
            CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertManyAsync(entities);
            await _quizAssignmentFormRepository.InsertManyAsync(quizAssignmentForms);
            await _quizAssignmentFormQuestionRepository.InsertManyAsync(quizAssignmentFormQuestions);

            var quizAssignmentFormDic = quizAssignmentForms.ToDictionary(x => x.Id);
            var quizAssignmentFormQuestionDic = quizAssignmentFormQuestions
                .GroupBy(x => x.QuizAssignmentFormId)
                .ToDictionary(x => x.Key, x => x.ToList());

            entities.ForEach(async entity => await SendAssignmentChangeEvent(
                entity,
                AssignmentChangeType.Created,
                quizAssignmentFormDic.GetValueOrDefault(entity.Id, new QuizAssignmentForm()),
                quizAssignmentFormQuestionDic.GetValueOrDefault(entity.Id, new List<QuizAssignmentFormQuestion>()),
                cancellationToken));
        }

        public async Task Update(Assignment entity, CreateOrUpdateAssignmentCommandQuizForm quizForm = null, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            var quizAssignmentFormModel = quizForm != null
                ? await UpdateQuizAssignmentForm(entity, quizForm)
                : await GetQuizAssignmentForm(entity);

            await SendAssignmentChangeEvent(entity, AssignmentChangeType.Updated, quizAssignmentFormModel, cancellationToken);
        }

        public async Task Delete(Assignment entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(entity);

            await DeleteAssignmentQuizFormData(entity, cancellationToken);

            await SendAssignmentChangeEvent(entity, AssignmentChangeType.Deleted);
        }

        private async Task DeleteAssignmentQuizFormData(Assignment entity, CancellationToken cancellationToken)
        {
            var aggregatedAssignment =
                (await _aggregatedContentSharedQuery.AssignmentByQuery(
                    _readAssignmentRepository.GetAll().Where(x => x.Id == entity.Id), true, false, cancellationToken)).First();

            await _quizAssignmentFormRepository.DeleteAsync(aggregatedAssignment.QuizForm);
            await _quizAssignmentFormQuestionRepository.DeleteManyAsync(aggregatedAssignment.QuizFormQuestions);
        }

        private QuizAssignmentFormQuestion SetDataForQuizAssignmentFormQuestion(QuizAssignmentFormQuestion source, QuizAssignmentFormQuestion target)
        {
            source.QuizAssignmentFormId = target.QuizAssignmentFormId;
            source.Priority = target.Priority;
            source.MaxScore = target.MaxScore;
            source.Question_Type = target.Question_Type;
            source.Question_Title = target.Question_Title;
            source.Question_CorrectAnswer = target.Question_CorrectAnswer;
            source.Question_Options = target.Question_Options?.Select(p => p.Clone());
            source.Question_Hint = target.Question_Hint;
            source.Question_AnswerExplanatoryNote = target.Question_AnswerExplanatoryNote;
            source.Question_FeedbackCorrectAnswer = target.Question_FeedbackCorrectAnswer;
            source.Question_FeedbackWrongAnswer = target.Question_FeedbackWrongAnswer;
            return source;
        }

        private async Task<QuizAssignmentFormModel> UpdateQuizAssignmentForm(Assignment assignment, CreateOrUpdateAssignmentCommandQuizForm quizForm)
        {
            var quizAssignmentForm = await _quizAssignmentFormRepository.FirstOrDefaultAsync(assignment.Id);

            if (quizAssignmentForm != null)
            {
                // Update configuration of assignment form
                quizAssignmentForm.RandomizedQuestions = quizForm.RandomizedQuestions;

                await _quizAssignmentFormRepository.UpdateAsync(quizAssignmentForm);

                var quizAssignmentFormQuestions = await _quizAssignmentFormQuestionRepository.GetAllListAsync(p => p.QuizAssignmentFormId == assignment.Id);

                if (quizForm.Questions != null)
                {
                    quizAssignmentFormQuestions.Update(
                        quizForm.Questions.Select(p => p.ToEntity(assignment.Id)),
                        p => p.Id,
                        (source, target) =>
                        {
                            _quizAssignmentFormQuestionRepository.Update(SetDataForQuizAssignmentFormQuestion(source, target));
                        },
                        target =>
                        {
                            target.Id = Guid.NewGuid();
                            _quizAssignmentFormQuestionRepository.Insert(target);
                        },
                        source =>
                        {
                            _quizAssignmentFormQuestionRepository.Delete(source.Id);
                        });
                }

                return new QuizAssignmentFormModel(
                    quizAssignmentForm,
                    quizForm.Questions != null ? quizForm.Questions.Select(p => p.ToEntity()) : quizAssignmentFormQuestions,
                    false);
            }

            return null;
        }

        private async Task SendAssignmentChangeEvent(
            Assignment assignment,
            AssignmentChangeType changeType,
            QuizAssignmentForm quizAssignmentForm = null,
            List<QuizAssignmentFormQuestion> quizAssignmentFormQuestions = null,
            CancellationToken cancellationToken = default)
        {
            var quizAssignmentFormModel = new QuizAssignmentFormModel(quizAssignmentForm, quizAssignmentFormQuestions, false);

            await SendAssignmentChangeEvent(assignment, changeType, quizAssignmentFormModel, cancellationToken);
        }

        private async Task SendAssignmentChangeEvent(
            Assignment assignment,
            AssignmentChangeType changeType,
            QuizAssignmentFormModel quizAssignmentFormModel,
            CancellationToken cancellationToken = default)
        {
            await ThunderCqrs.SendEvent(
                new AssignmentChangeEvent(new AssignmentModel(assignment, quizAssignmentFormModel), changeType),
                cancellationToken);
        }

        private async Task<QuizAssignmentFormModel> GetQuizAssignmentForm(Assignment assignment)
        {
            var quizAssignmentForm = await _quizAssignmentFormRepository.FirstOrDefaultAsync(assignment.Id);
            var quizAssignmentFormQuestions = await _quizAssignmentFormQuestionRepository.GetAllListAsync(p => p.QuizAssignmentFormId == assignment.Id);

            return quizAssignmentForm != null
                ? new QuizAssignmentFormModel(quizAssignmentForm, quizAssignmentFormQuestions, false)
                : null;
        }

        private async Task<QuizAssignmentFormModel> CreateQuizAssignmentForm(Assignment entity, CreateOrUpdateAssignmentCommandQuizForm quizForm)
        {
            QuizAssignmentFormModel quizAssignmentFormModel = null;

            if (quizForm != null)
            {
                var newQuizAssignmentForm = new QuizAssignmentForm { Id = entity.Id };
                var newQuizAssignmentFormQuestions = quizForm.Questions
                    .Select(x => SetDataForQuizAssignmentFormQuestion(
                        new QuizAssignmentFormQuestion() { Id = Guid.NewGuid() },
                        x.ToEntity(entity.Id)))
                    .ToList();

                await _quizAssignmentFormRepository.InsertAsync(new QuizAssignmentForm
                { Id = entity.Id, RandomizedQuestions = quizForm.RandomizedQuestions });
                await _quizAssignmentFormQuestionRepository.InsertManyAsync(newQuizAssignmentFormQuestions);

                quizAssignmentFormModel =
                    new QuizAssignmentFormModel(newQuizAssignmentForm, newQuizAssignmentFormQuestions, false);
            }

            return quizAssignmentFormModel;
        }
    }
}
