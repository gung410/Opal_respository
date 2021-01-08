using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class UpdateSurveyParticipantStatusCommandHandler : BaseCommandHandler<UpdateSurveyParticipantStatusCommand>
    {
        private readonly IRepository<SurveyParticipant> _formParticipantRepository;
        private readonly IRepository<SurveyAnswer> _formAnswerRepository;

        public UpdateSurveyParticipantStatusCommandHandler(
            IAccessControlContext accessControlContext,
            IRepository<SurveyParticipant> formParticipantRepository,
            IRepository<SurveyAnswer> formAnswerRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _formParticipantRepository = formParticipantRepository;
            _formAnswerRepository = formAnswerRepository;
        }

        protected override async Task HandleAsync(UpdateSurveyParticipantStatusCommand command, CancellationToken cancellationToken)
        {
            var answer = _formAnswerRepository.FirstOrDefault(
                m => m.FormId == command.FormId && m.OwnerId == command.CurrentUserId);

            if (answer == null)
            {
                throw new EntityNotFoundException($"There is no answer of current request.");
            }

            var currentParticipant = await _formParticipantRepository.FirstOrDefaultAsync(
                m => m.SurveyId == command.FormId && m.UserId == command.CurrentUserId);

            if (currentParticipant == null)
            {
                throw new EntityNotFoundException($"There is no participant of current request.");
            }

            currentParticipant.Status = answer.IsCompleted ? SurveyParticipantStatus.Completed : SurveyParticipantStatus.Incomplete;
            await _formParticipantRepository.UpdateAsync(currentParticipant);
        }
    }
}
