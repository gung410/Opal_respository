using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Domain.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Form.Application.Commands
{
    public class DeleteQuestionBankCommandHandler : BaseCommandHandler<DeleteQuestionBankCommand>
    {
        private readonly IRepository<QuestionBank> _questionBankRepository;
        private readonly IRepository<QuestionGroup> _questionGroupRepository;

        public DeleteQuestionBankCommandHandler(
            IAccessControlContext accessControlContext,
            IRepository<QuestionBank> questionBankRepository,
            IRepository<QuestionGroup> questionGroupRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _questionBankRepository = questionBankRepository;
            _questionGroupRepository = questionGroupRepository;
        }

        protected override async Task HandleAsync(DeleteQuestionBankCommand command, CancellationToken cancellationToken)
        {
            var toDeletedQuestion = await _questionBankRepository.GetAsync(command.QuestionBankId);
            if (toDeletedQuestion is null)
            {
                throw new EntityNotFoundException($"{nameof(toDeletedQuestion)} not found");
            }

            var hasOwnerPermission = toDeletedQuestion.CreatedBy == CurrentUserId;
            if (hasOwnerPermission == false)
            {
                throw new QuestionBankAccessDeniedException();
            }

            if (toDeletedQuestion.QuestionGroupId.HasValue)
            {
                var isQuestionGroupInUse = _questionBankRepository
                                                .GetAll()
                                                .Any(question => question.Id != toDeletedQuestion.Id
                                                              && question.QuestionGroupId == toDeletedQuestion.QuestionGroupId);

                if (isQuestionGroupInUse == false)
                {
                    await _questionGroupRepository.DeleteAsync(toDeletedQuestion.QuestionGroupId.Value);
                }
            }

            await _questionBankRepository.DeleteAsync(toDeletedQuestion);
        }
    }
}
