using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Commands;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Common.Extensions;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Microservice.LnaForm.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormStandaloneByIdQueryHandler : BaseQueryHandler<GetFormStandaloneByIdQuery, FormWithQuestionsModel>
    {
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormSection> _formSectionRepository;
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public GetFormStandaloneByIdQueryHandler(
            IThunderCqrs thunderCqrs,
            IRepository<FormQuestion> formQuestionRepository,
            IRepository<AccessRight> accessRightRepository,
            IRepository<FormEntity> formRepository,
            IRepository<FormSection> formSectionRepository,
            IRepository<FormParticipant> formParticipantRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _formQuestionRepository = formQuestionRepository;
            _accessRightRepository = accessRightRepository;
            _formRepository = formRepository;
            _formSectionRepository = formSectionRepository;
            _formParticipantRepository = formParticipantRepository;
        }

        protected override async Task<FormWithQuestionsModel> HandleAsync(GetFormStandaloneByIdQuery query, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository
                .GetAll()
                .ApplyAccessControlEx(
                    AccessControlContext,
                    LnaFormEntityExpressions.HasPermissionToSeeFormExpr(CurrentUserId))
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId);

            // Get latest published version
            var form = await formQuery
                .Where(p => p.OriginalObjectId == query.FormOriginalObjectId && p.Status == FormStatus.Published && !p.IsArchived)
                .FirstOrDefaultAsync(cancellationToken);

            if (form == null)
            {
                throw new FormAccessDeniedException();
            }

            var now = Clock.Now;
            var isValidStartDate = !form.StartDate.HasValue || form.StartDate.Value <= now;
            var isValidEndDate = !form.EndDate.HasValue || form.EndDate.Value > now;
            if (!isValidStartDate || !isValidEndDate)
            {
                throw new FormAccessDeniedException();
            }

            await CheckValidParticipantAsync(CurrentUserId, form.OriginalObjectId, form.Id);

            var canUnpublishStandalone = await _formParticipantRepository
                            .CountAsync(m => m.FormId == form.Id && m.Status == FormParticipantStatus.Completed) == 0;

            var formQuestions = await _formQuestionRepository
                .GetAll()
                .Where(question => question.FormId == form.Id)
                .OrderBy(question => question.Priority)
                .ThenBy(question => question.MinorPriority)
                .ToListAsync(cancellationToken);

            var formSections = await _formSectionRepository.GetAll().Where(section => section.FormId == form.Id).ToListAsync();

            return new FormWithQuestionsModel(form, formQuestions, formSections, canUnpublishStandalone);
        }

        private async Task CheckValidParticipantAsync(Guid currentUserId, Guid formOriginalObjectId, Guid formId)
        {
            var currentParticipant = await _formParticipantRepository.FirstOrDefaultAsync(m => m.UserId == currentUserId && m.FormOriginalObjectId == formOriginalObjectId);
            if (currentParticipant != null)
            {
                if (currentParticipant.FormId != formId)
                {
                    currentParticipant.FormId = formId;
                }

                if (!currentParticipant.IsStarted.HasValue || !currentParticipant.IsStarted.Value)
                {
                    currentParticipant.IsStarted = true;
                    currentParticipant.Status = FormParticipantStatus.Incomplete;
                }

                await _formParticipantRepository.UpdateAsync(currentParticipant);
            }
            else
            {
                AssignFormParticipantCommand assignCommand = new AssignFormParticipantCommand()
                {
                    CurrentUserId = currentUserId,
                    FormOriginalObjectId = formOriginalObjectId,
                    FormId = formId,
                    IsStarted = true,
                    Status = FormParticipantStatus.Incomplete,
                    UserIds = new List<Guid>()
                        {
                            currentUserId
                        }
                };
                await _thunderCqrs.SendCommand(assignCommand);
            }
        }
    }
}
