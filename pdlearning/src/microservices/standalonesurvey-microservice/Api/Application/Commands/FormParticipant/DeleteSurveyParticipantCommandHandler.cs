using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.Services;
using Microservice.StandaloneSurvey.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class DeleteSurveyParticipantCommandHandler : BaseCommandHandler<DeleteFormParticipantCommand>
    {
        private readonly IRepository<SurveyParticipant> _formParticipantRepository;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<SyncedUser> _userRepository;
        private readonly SurveyParticipantNotifyApplicationService _surveyParticipantNotifyApplicationService;

        public DeleteSurveyParticipantCommandHandler(
            IRepository<SurveyParticipant> formParticipantRepository,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IRepository<SyncedUser> userRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext,
            SurveyParticipantNotifyApplicationService surveyParticipantNotifyApplicationService) : base(unitOfWorkManager, accessControlContext)
        {
            _formParticipantRepository = formParticipantRepository;
            _formRepository = formRepository;
            _userRepository = userRepository;
            _surveyParticipantNotifyApplicationService = surveyParticipantNotifyApplicationService;
        }

        protected override async Task HandleAsync(DeleteFormParticipantCommand command, CancellationToken cancellationToken)
        {
            var existedForm = await _formRepository
                .FirstOrDefaultAsync(p => p.Id == command.FormId);

            if (existedForm == null)
            {
                throw new SurveyAccessDeniedException();
            }

            var participants = await _formParticipantRepository
                .GetAllListAsync(_ => command.Ids.Contains(_.Id));

            await _formParticipantRepository.DeleteManyAsync(participants);

            await PerformNotifyRemoveParticipant(participants, existedForm);
        }

        private async Task PerformNotifyRemoveParticipant(List<SurveyParticipant> participants, Domain.Entities.StandaloneSurvey form)
        {
            var formOwnerName = _userRepository
                .FirstOrDefault(p => p.Id == form.OwnerId)
                .FullName();

            var participantIds = participants.Select(p => p.UserId).ToList();

            var participantInfo = _userRepository
                .GetAll()
                .Where(p => participantIds.Contains(p.Id))
                .Select(p => new { p.Id, FullName = p.FullName() })
                .ToList();

            foreach (var user in participantInfo)
            {
                await _surveyParticipantNotifyApplicationService.NotifyRemovedFormParticipant(
                  new NotifyFormParticipantModel
                  {
                      FormOriginalObjectId = form.OriginalObjectId,
                      FormOwnerName = formOwnerName,
                      FormTitle = form.Title,
                      ParcitipantId = user.Id,
                      ParticipantName = user.FullName
                  });
            }
        }
    }
}
