using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class AssignSurveyParticipantCommandHandler : BaseCommandHandler<AssignSurveyParticipantCommand>
    {
        private readonly IRepository<SurveyParticipant> _formParticipantRepository;

        public AssignSurveyParticipantCommandHandler(
            IAccessControlContext accessControlContext,
            IRepository<SurveyParticipant> formParticipantRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _formParticipantRepository = formParticipantRepository;
        }

        protected override async Task HandleAsync(AssignSurveyParticipantCommand command, CancellationToken cancellationToken)
        {
            if (!command.UserIds.Any())
            {
                throw new EntityNotFoundException($"There is no participants in the {nameof(AssignSurveyParticipantCommand)}.");
            }

            var assignedParticipants = _formParticipantRepository
               .GetAll()
               .Where(x => command.UserIds.Contains(x.UserId) && x.SurveyId == command.SurveyId)
               .Select(x => x.UserId)
               .ToList();

            var newParticipant = command.UserIds
                .Where(uid => !assignedParticipants.Contains(uid))
                .Select(uid => new SurveyParticipant
                {
                    Id = Guid.NewGuid(),
                    UserId = uid,
                    SurveyId = command.SurveyId,
                    SurveyOriginalObjectId = command.SurveyOriginalObjectId,
                    IsStarted = command.IsStarted,
                    AssignedDate = Clock.Now,
                    CreatedBy = command.CurrentUserId,
                    Status = command.Status ?? SurveyParticipantStatus.NotStarted
                })
                .ToList();

            await _formParticipantRepository.InsertManyAsync(newParticipant);
        }
    }
}
