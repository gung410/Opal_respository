using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Common.Extensions;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Microservice.StandaloneSurvey.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormWithQuestionsByIdQueryHandler : BaseQueryHandler<GetFormWithQuestionsByIdQuery, SurveyWithQuestionsModel>
    {
        private readonly IRepository<SurveyQuestion> _formQuestionRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<SurveySection> _formSectionRepository;
        private readonly IRepository<SurveyParticipant> _formParticipantRepository;
        private readonly ICslAccessControlContext _cslAccessControlContext;

        public GetFormWithQuestionsByIdQueryHandler(
            IRepository<SurveyQuestion> formQuestionRepository,
            IRepository<AccessRight> accessRightRepository,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IRepository<SurveySection> formSectionRepository,
            IRepository<SurveyParticipant> formParticipantRepository,
            IAccessControlContext accessControlContext,
            ICslAccessControlContext cslAccessControlContext) : base(accessControlContext)
        {
            _formQuestionRepository = formQuestionRepository;
            _accessRightRepository = accessRightRepository;
            _formRepository = formRepository;
            _formSectionRepository = formSectionRepository;
            _formParticipantRepository = formParticipantRepository;
            _cslAccessControlContext = cslAccessControlContext;
        }

        protected override async Task<SurveyWithQuestionsModel> HandleAsync(GetFormWithQuestionsByIdQuery query, CancellationToken cancellationToken)
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

            var form = await formQuery
                .Where(p => p.Id == query.FormId)
                .WhereIf(query.OnlyPublished, p => p.Status == SurveyStatus.Published)
                .FirstOrDefaultAsync(cancellationToken);

            if (form == null)
            {
                throw new SurveyAccessDeniedException();
            }

            var canUnpublishStandalone = await _formParticipantRepository
                        .CountAsync(m => m.SurveyOriginalObjectId == form.OriginalObjectId && m.Status == SurveyParticipantStatus.Completed) == 0;

            var formQuestions = await _formQuestionRepository
                .GetAll()
                .Where(question => question.SurveyId == query.FormId)
                .OrderBy(question => question.Priority)
                .ThenBy(question => question.MinorPriority)
                .ToListAsync(cancellationToken);

            var formSections = await _formSectionRepository.GetAll().Where(section => section.SurveyId == query.FormId).ToListAsync();

            return new SurveyWithQuestionsModel(form, formQuestions, formSections, canUnpublishStandalone);
        }
    }
}
