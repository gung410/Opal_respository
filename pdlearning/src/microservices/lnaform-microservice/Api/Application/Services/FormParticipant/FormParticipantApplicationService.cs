using System;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Microservice.LnaForm.Application.Commands;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.Queries;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services.FormParticipant.Dtos;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;
using FormParticipantEntity = Microservice.LnaForm.Domain.Entities.FormParticipant;

namespace Microservice.LnaForm.Application.Services
{
    public class FormParticipantApplicationService : BaseApplicationService
    {
        private readonly IRepository<FormParticipantEntity> _formParticipantRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly FormParticipantNotifyApplicationService _formParticipantNotifyApplicationService;

        public FormParticipantApplicationService(
            IRepository<FormParticipantEntity> formParticipantRepository,
            IRepository<UserEntity> userRepository,
            IRepository<FormEntity> formRepository,
            FormParticipantNotifyApplicationService formParticipantNotifyApplicationService,
            IThunderCqrs thunderCqrs) : base(thunderCqrs)
        {
            _formRepository = formRepository;
            _formParticipantRepository = formParticipantRepository;
            _formParticipantNotifyApplicationService = formParticipantNotifyApplicationService;
            _userRepository = userRepository;
        }

        public async Task AssignFormParticipant(AssignFormParticipantsDto request, Guid userId)
        {
            var saveCommand = new AssignFormParticipantCommand
            {
                FormOriginalObjectId = request.FormOriginalObjectId,
                FormId = request.FormId,
                UserIds = request.UserIds,
                CurrentUserId = userId,
            };
            await this.ThunderCqrs.SendCommand(saveCommand);
        }

        public async Task UpdateFormParticipantStatus(UpdateFormParticipantStatusDto request, Guid userId)
        {
            var saveCommand = new UpdateFormParticipantStatusCommand
            {
                FormId = request.FormId,
                CurrentUserId = userId
            };
            await this.ThunderCqrs.SendCommand(saveCommand);
        }

        public Task<PagedResultDto<FormParticipantModel>> GetFormParticipantsByFormId(GetFormParticipantsByFormIdDto dto)
        {
            return this.ThunderCqrs.SendQuery(new GetFormParticipantsByFormIdQuery() { FormOriginalObjectId = dto.FormOriginalObjectId, PagedInfo = dto.PagedInfo });
        }

        public Task DeleteFormParticipantsById(DeleteFormParticipantsDto dto)
        {
            return this.ThunderCqrs.SendCommand(new DeleteFormParticipantCommand() { Ids = dto.Ids, FormId = dto.FormId });
        }

        public async Task RemindFormParticipant(RemindFormParticipantRequest request)
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
                throw new FormAccessDeniedException();
            }

            foreach (var user in existedParticipantInfo)
            {
                await _formParticipantNotifyApplicationService.NotifyReminderedFormParticipant(
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
