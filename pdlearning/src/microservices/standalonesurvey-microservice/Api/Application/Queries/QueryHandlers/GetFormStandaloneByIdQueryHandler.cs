using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Commands;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Common.Extensions;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Microservice.StandaloneSurvey.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormStandaloneByIdQueryHandler : BaseQueryHandler<GetFormStandaloneByIdQuery, SurveyWithQuestionsModel>
    {
        private readonly IRepository<SurveyQuestion> _formQuestionRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<SurveySection> _formSectionRepository;
        private readonly IRepository<SurveyParticipant> _formParticipantRepository;
        private readonly ICslAccessControlContext _cslAccessControlContext;
        private readonly IThunderCqrs _thunderCqrs;

        public GetFormStandaloneByIdQueryHandler(
            IThunderCqrs thunderCqrs,
            IRepository<SurveyQuestion> formQuestionRepository,
            IRepository<AccessRight> accessRightRepository,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IRepository<SurveySection> formSectionRepository,
            IRepository<SurveyParticipant> formParticipantRepository,
            IAccessControlContext accessControlContext,
            ICslAccessControlContext cslAccessControlContext) : base(accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _formQuestionRepository = formQuestionRepository;
            _accessRightRepository = accessRightRepository;
            _formRepository = formRepository;
            _formSectionRepository = formSectionRepository;
            _formParticipantRepository = formParticipantRepository;
            _cslAccessControlContext = cslAccessControlContext;
        }

        protected override async Task<SurveyWithQuestionsModel> HandleAsync(GetFormStandaloneByIdQuery query, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository.GetAll();

            if (query.SubModule == SubModule.Lna)
            {
                formQuery = formQuery
                    .ApplyAccessControlEx(
                        AccessControlContext,
                        SurveyEntityExpressions.HasPermissionToSeeFormExpr(CurrentUserId))
                    .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId);
            }
            else
            {
                formQuery = formQuery
                    .ApplyCslAccessControl(
                        _cslAccessControlContext,
                        roles: SurveyEntityExpressions.AllViewableCslRoles(),
                        communityId: query.CommunityId,
                        includePredicate: SurveyEntityExpressions.FilterCslSurveyPublishedExpr());
            }

            // Get latest published version
            var form = await formQuery
                .Where(p => p.OriginalObjectId == query.FormOriginalObjectId && p.Status == SurveyStatus.Published && !p.IsArchived)
                .FirstOrDefaultAsync(cancellationToken);

            if (form == null)
            {
                throw new SurveyAccessDeniedException();
            }

            var now = Clock.Now;
            var isValidStartDate = !form.StartDate.HasValue || form.StartDate.Value <= now;
            var isValidEndDate = !form.EndDate.HasValue || form.EndDate.Value > now;
            if (!isValidStartDate || !isValidEndDate)
            {
                throw new SurveyAccessDeniedException();
            }

            await CheckValidParticipantAsync(CurrentUserId, form.OriginalObjectId, form.Id);

            var canUnpublishStandalone = await _formParticipantRepository
                            .CountAsync(m => m.SurveyId == form.Id && m.Status == SurveyParticipantStatus.Completed) == 0;

            var formQuestions = await _formQuestionRepository
                .GetAll()
                .Where(question => question.SurveyId == form.Id)
                .OrderBy(question => question.Priority)
                .ThenBy(question => question.MinorPriority)
                .ToListAsync(cancellationToken);

            var formSections = await _formSectionRepository.GetAll().Where(section => section.SurveyId == form.Id).ToListAsync();

            return new SurveyWithQuestionsModel(form, formQuestions, formSections, canUnpublishStandalone);
        }

        private async Task CheckValidParticipantAsync(Guid currentUserId, Guid formOriginalObjectId, Guid formId)
        {
            var currentParticipant = await _formParticipantRepository.FirstOrDefaultAsync(m => m.UserId == currentUserId && m.SurveyOriginalObjectId == formOriginalObjectId);
            if (currentParticipant != null)
            {
                if (currentParticipant.SurveyId != formId)
                {
                    currentParticipant.SurveyId = formId;
                }

                if (!currentParticipant.IsStarted.HasValue || !currentParticipant.IsStarted.Value)
                {
                    currentParticipant.IsStarted = true;
                    currentParticipant.Status = SurveyParticipantStatus.Incomplete;
                }

                await _formParticipantRepository.UpdateAsync(currentParticipant);
            }
            else
            {
                AssignSurveyParticipantCommand assignCommand = new AssignSurveyParticipantCommand()
                {
                    CurrentUserId = currentUserId,
                    SurveyOriginalObjectId = formOriginalObjectId,
                    SurveyId = formId,
                    IsStarted = true,
                    Status = SurveyParticipantStatus.Incomplete,
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
