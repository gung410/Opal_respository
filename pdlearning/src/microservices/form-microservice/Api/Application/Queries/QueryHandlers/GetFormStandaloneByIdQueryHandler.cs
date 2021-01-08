using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Commands;
using Microservice.Form.Application.Events;
using Microservice.Form.Application.Models;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Queries
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
                .GetAllWithAccessControl(AccessControlContext, FormEntityExpressions.HasPermissionToSeeFormExpr(CurrentUserId))
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                .CombineWithPublicSurveyTemplates(_formRepository);

            // Get latest published version
            var form = await formQuery
                .Where(p => p.OriginalObjectId == query.FormOriginalObjectId && p.Status == FormStatus.Published && !p.IsArchived)
                .FirstOrDefaultAsync(cancellationToken);

            if (form == null || !form.IsStandalone.HasValue || !form.IsStandalone.Value)
            {
                throw new FormAccessDeniedException();
            }

            var now = DateTime.UtcNow.Date;
            var isValidStartDate = !form.StartDate.HasValue || form.StartDate.Value.Date <= now;
            var isValidEndDate = !form.EndDate.HasValue || form.EndDate.Value.Date >= now;
            if (!isValidStartDate || !isValidEndDate)
            {
                throw new FormAccessDeniedException();
            }

            await CheckValidParticipantAsync(CurrentUserId, form.OriginalObjectId, form.Id, form.StandaloneMode);

            var canUnpublishStandalone = _formParticipantRepository.Count(m => m.FormId == form.Id && m.Status == FormParticipantStatus.Completed) == 0;

            var formQuestions = await _formQuestionRepository
                .GetAll()
                .Where(question => question.FormId == form.Id)
                .OrderBy(question => question.Priority)
                .ThenBy(question => question.MinorPriority)
                .ToListAsync(cancellationToken);

            var formSections = await _formSectionRepository
                .GetAll()
                .Where(section => section.FormId == form.Id)
                .ToListAsync(cancellationToken);

            return new FormWithQuestionsModel(form, formQuestions, formSections, canUnpublishStandalone);
        }

        private async Task CheckValidParticipantAsync(Guid currentUserId, Guid formOriginalObjectId, Guid formId, FormStandaloneMode? formStandaloneMode)
        {
            var currentParticipant = await _formParticipantRepository
                .GetAll()
                .Where(m => m.UserId == currentUserId)
                .Where(m => m.FormOriginalObjectId == formOriginalObjectId)
                .FirstOrDefaultAsync();

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

                await _thunderCqrs.SendEvent(
                    new FormParticipantChangeEvent(currentParticipant, FormParticipantChangeType.Updated));
            }
            else
            {
                if (formStandaloneMode == FormStandaloneMode.Restricted)
                {
                    throw new FormStandaloneAccessDeniedException();
                }

                if (formStandaloneMode == FormStandaloneMode.Public)
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
}
