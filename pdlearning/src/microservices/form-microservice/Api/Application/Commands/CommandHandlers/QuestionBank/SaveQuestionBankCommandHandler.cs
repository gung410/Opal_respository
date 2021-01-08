using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Questions;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class SaveQuestionBankCommandHandler : BaseCommandHandler<SaveQuestionBankCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<QuestionBank> _questionBankRepository;
        private readonly IRepository<QuestionGroup> _questionGroupRepository;

        public SaveQuestionBankCommandHandler(
            IRepository<QuestionBank> questionBankRepository,
            IRepository<QuestionGroup> questionGroupRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext,
            IThunderCqrs thunderCqrs) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _questionBankRepository = questionBankRepository;
            _questionGroupRepository = questionGroupRepository;
        }

        protected override async Task HandleAsync(SaveQuestionBankCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreation)
            {
                await Create(command);
            }
            else
            {
                await Update(command);
            }
        }

        private async Task Create(SaveQuestionBankCommand command)
        {
            var questionGroupId = await GetQuestionGroupId(command.QuestionGroupName);
            var questionBank = command.ToQuestionBank(questionGroupId);
            await _questionBankRepository.InsertAsync(questionBank);
        }

        private async Task Update(SaveQuestionBankCommand command)
        {
            var existingQuestionBank = await _questionBankRepository.GetAsync(command.Id);
            if (existingQuestionBank is null)
            {
                throw new EntityNotFoundException($"{nameof(existingQuestionBank)} not found");
            }

            var newQuestionGroupId = await GetQuestionGroupId(command.QuestionGroupName);
            if (existingQuestionBank.QuestionGroupId.HasValue && existingQuestionBank.QuestionGroupId != newQuestionGroupId)
            {
                var usedByOtherQuestion = _questionBankRepository
                    .GetAll()
                    .Any(
                        questionBank => questionBank.Id != existingQuestionBank.Id &&
                            questionBank.QuestionGroupId == existingQuestionBank.QuestionGroupId);
                if (!usedByOtherQuestion)
                {
                    await _questionGroupRepository.DeleteAsync(existingQuestionBank.QuestionGroupId.Value);
                }
            }

            existingQuestionBank.Id = command.Id;
            existingQuestionBank.IsDeleted = command.IsDeleted;
            existingQuestionBank.IsScoreEnabled = command.IsScoreEnabled;
            existingQuestionBank.QuestionAnswerExplanatoryNote = command.QuestionAnswerExplanatoryNote;
            existingQuestionBank.QuestionCorrectAnswer = command.QuestionCorrectAnswer;
            existingQuestionBank.QuestionFeedbackCorrectAnswer = command.QuestionFeedbackCorrectAnswer;
            existingQuestionBank.QuestionFeedbackWrongAnswer = command.QuestionFeedbackWrongAnswer;
            existingQuestionBank.QuestionGroupId = newQuestionGroupId;
            existingQuestionBank.QuestionHint = command.QuestionHint;
            existingQuestionBank.QuestionLevel = command.QuestionLevel;
            existingQuestionBank.QuestionOptions = command.QuestionOptions?.Select(p => (QuestionOption)p);
            existingQuestionBank.QuestionTitle = command.QuestionTitle;
            existingQuestionBank.QuestionType = command.QuestionType;
            existingQuestionBank.RandomizedOptions = command.RandomizedOptions;
            existingQuestionBank.Score = command.Score;
            existingQuestionBank.Title = command.Title;
            existingQuestionBank.ChangedBy = command.UserId;

            await _questionBankRepository.UpdateAsync(existingQuestionBank);
        }

        private async Task<Guid?> GetQuestionGroupId(string questionGroupName)
        {
            if (string.IsNullOrWhiteSpace(questionGroupName))
            {
                return null;
            }

            var questionGroup = await _questionGroupRepository.FirstOrDefaultAsync(questionGroup => questionGroup.Name == questionGroupName);
            if (questionGroup != null)
            {
                return questionGroup.Id;
            }
            else
            {
                var newQuestionGroup = new QuestionGroup
                {
                    Name = questionGroupName
                };
                return await _questionGroupRepository.InsertAndGetIdAsync(newQuestionGroup);
            }
        }
    }
}
