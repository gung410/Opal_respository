using System;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Microservice.StandaloneSurvey.Application.Commands;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.Queries;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Microservice.StandaloneSurvey.Application.Services.SurveyParticipant.Dtos;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Services
{
    public class SurveyParticipantApplicationService : BaseApplicationService
    {
        private readonly IRepository<Domain.Entities.SurveyParticipant> _formParticipantRepository;
        private readonly IRepository<SyncedUser> _userRepository;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly SurveyParticipantNotifyApplicationService _surveyParticipantNotifyApplicationService;

        public SurveyParticipantApplicationService(
            IRepository<Domain.Entities.SurveyParticipant> formParticipantRepository,
            IRepository<SyncedUser> userRepository,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            SurveyParticipantNotifyApplicationService surveyParticipantNotifyApplicationService,
            IThunderCqrs thunderCqrs) : base(thunderCqrs)
        {
            _formRepository = formRepository;
            _formParticipantRepository = formParticipantRepository;
            _surveyParticipantNotifyApplicationService = surveyParticipantNotifyApplicationService;
            _userRepository = userRepository;
        }

        public async Task AssignSurveyParticipant(AssignSurveyParticipantsDto request, Guid userId)
        {
            var saveCommand = new AssignSurveyParticipantCommand
            {
                SurveyOriginalObjectId = request.FormOriginalObjectId,
                SurveyId = request.FormId,
                UserIds = request.UserIds,
                CurrentUserId = userId,
            };
            await this.ThunderCqrs.SendCommand(saveCommand);
        }

        public async Task UpdateSurveyParticipantStatus(UpdateSurveyParticipantStatusDto request, Guid userId)
        {
            var saveCommand = new UpdateSurveyParticipantStatusCommand
            {
                FormId = request.FormId,
                CurrentUserId = userId
            };
            await this.ThunderCqrs.SendCommand(saveCommand);
        }

        public Task<PagedResultDto<SurveyParticipantModel>> GetSurveyParticipantsByFormId(GetSurveyParticipantsByFormIdDto dto)
        {
            return this.ThunderCqrs.SendQuery(new GetFormParticipantsByFormIdQuery() { FormOriginalObjectId = dto.FormOriginalObjectId, PagedInfo = dto.PagedInfo });
        }

        public Task DeleteSurveyParticipantsById(DeleteSurveyParticipantsDto dto)
        {
            return this.ThunderCqrs.SendCommand(new DeleteFormParticipantCommand() { Ids = dto.Ids, FormId = dto.FormId });
        }

        public async Task RemindSurveyParticipant(RemindSurveyParticipantRequest request)
        {
            var exsitedForm = await _formRepository.FirstOrDefaultAsync(x => x.Id == request.FormId);
            string formOwnerName = _userRepository.FirstOrDefault(p => p.Id == exsitedForm.OwnerId).FullName();

            var existedParticipantInfo = await _formParticipantRepository
                                           .GetAll()
                                           .Where(p => request.ParticipantIds.Contains(p.Id))
                                           .Join(
                                                _userRepository.GetAll(),
                                                participantInfo => participantInfo.UserId,
                                                userInfo => userInfo.Id,
                                                (participant, user) => new { participant.UserId, fullName = user.FullName() })
                                           .ToListAsync();

            if (exsitedForm == null || existedParticipantInfo.Count == 0)
            {
                throw new SurveyAccessDeniedException();
            }

            foreach (var user in existedParticipantInfo)
            {
                await _surveyParticipantNotifyApplicationService.NotifyReminderedFormParticipant(
                          new NotifyFormParticipantModel
                          {
                              FormOriginalObjectId = exsitedForm.OriginalObjectId,
                              FormOwnerName = formOwnerName,
                              FormTitle = exsitedForm.Title,
                              ParcitipantId = user.UserId,
                              ParticipantName = user.fullName
                          });
            }
        }
    }
}
