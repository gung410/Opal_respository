using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Microservice.Form.Application.Commands;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.Queries;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services.FormParticipant.Dtos;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.Form.Domain.Entities.Form;
using FormParticipantEntity = Microservice.Form.Domain.Entities.FormParticipant;

namespace Microservice.Form.Application.Services
{
    public class FormParticipantApplicationService : BaseApplicationService
    {
        private readonly IRepository<FormParticipantEntity> _formParticipantRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly FormNotifyApplicationService _formParticipantNotifyApplicationService;

        public FormParticipantApplicationService(
            IRepository<FormParticipantEntity> formParticipantRepository,
            IRepository<UserEntity> userRepository,
            IRepository<FormEntity> formRepository,
            FormNotifyApplicationService formParticipantNotifyApplicationService,
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

        public Task<FormParticipantModel> GetMyParticipantData(Guid formId)
        {
            return this.ThunderCqrs.SendQuery(new GetMyParticipantDataByFormIdQuery() { FormOriginalObjectId = formId });
        }

        public Task<IEnumerable<FormParticipantFormModel>> GetFormParticipantsByFormIds(GetFormParticipantsByFormIdsDto dto)
        {
            return this.ThunderCqrs.SendQuery(new GetFormParticipantsByFormIdsQuery() { FormIds = dto.FormIds });
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
