using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Models;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Domain.Constants;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Infrastructure;
using Microservice.Form.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Helpers;
using Thunder.Service.Authentication;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Queries
{
    public class SearchFormsQueryHandler : BaseQueryHandler<SearchFormsQuery, PagedResultDto<FormModel>>
    {
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly IUserContext _userContext;

        public SearchFormsQueryHandler(
            IAccessControlContext accessControlContext,
            IRepository<AccessRight> accessRightRepository,
            IRepository<FormEntity> formRepository,
            IRepository<FormParticipant> formParticipantRepository,
            IUserContext userContext) : base(accessControlContext)
        {
            _formRepository = formRepository;
            _accessRightRepository = accessRightRepository;
            _formParticipantRepository = formParticipantRepository;
            _userContext = userContext;
        }

        protected override async Task<PagedResultDto<FormModel>> HandleAsync(SearchFormsQuery query, CancellationToken cancellationToken)
        {
            var fiveWorkingDayAgo = DateTimeHelper.GetDayBeforeXWorkingDayFromNow(5);
            var unionExpression = FormEntityExpressions.HasOwnerOrApprovalPermissionExpr(CurrentUserId);

            if (query.IncludeFormForImportToCourse)
            {
                unionExpression = FormEntityExpressions.AvailableForImportToCourseExpr(CurrentUserId);
            }

            var formQuery = _formRepository
                .GetAllWithAccessControl(AccessControlContext, unionExpression)
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                .IgnoreArchivedItems()
                .Where(f => f.IsSurveyTemplate != true);

            if (_userContext.IsInRole(UserRoles.CourseContentCreator)
                || _userContext.IsInRole(UserRoles.ContentCreator)
                || _userContext.IsInRole(UserRoles.CourseFacilitator)
                || _userContext.IsAdministrator())
            {
                formQuery = formQuery.CombineWithPublicSurveyTemplates(_formRepository);
                formQuery = formQuery.WhereIf(
                    query.IsSurveyTemplate != null,
                    f => f.IsSurveyTemplate == query.IsSurveyTemplate);
            }

            formQuery = formQuery.WhereIf(
                !string.IsNullOrEmpty(query.SearchFormTitle),
                f => EF.Functions.Like(f.Title, $"%{query.SearchFormTitle}%"));

            formQuery = formQuery.WhereIf(
                !query.FilterByStatus.IsNullOrEmpty(),
                f => query.FilterByStatus.Contains(f.Status));

            formQuery = formQuery.WhereIf(
                 query.FilterByType != null,
                 f => f.Type == query.FilterByType);

            formQuery = formQuery.WhereIf(
                 query.FilterBySurveyTypes != null && query.FilterBySurveyTypes.Any(),
                 f => f.SurveyType != null && query.FilterBySurveyTypes.Contains(f.SurveyType.Value));

            formQuery = formQuery.WhereIf(
                 query.ExcludeBySurveyTypes != null && query.ExcludeBySurveyTypes.Any(),
                 f => f.SurveyType == null || !query.ExcludeBySurveyTypes.Contains(f.SurveyType.Value));

            if (query.FilterByStatus.Contains(FormStatus.PendingApproval))
            {
                formQuery = formQuery.Where(r =>
                           (r.AlternativeApprovingOfficerId == query.UserId) ?
                           (r.Status == FormStatus.PendingApproval && r.SubmitDate <= fiveWorkingDayAgo)
                         : (r.Status == FormStatus.PendingApproval));
            }

            if (query.FilterByStatus.IsNullOrEmpty())
            {
                formQuery = formQuery.Where(r =>
                      (r.Status == FormStatus.PendingApproval && r.AlternativeApprovingOfficerId == query.UserId) ?
                      (r.SubmitDate <= fiveWorkingDayAgo) : (r.Status != FormStatus.Archived));
            }

            formQuery = formQuery.PrioritizeSurveyTemplate();

            formQuery = ApplyPaging(formQuery, query.PagedInfo);

            var totalCount = await formQuery.CountAsync(cancellationToken);

            var entities = await formQuery.Select(p => new FormModel(p)).ToListAsync(cancellationToken);

            entities = entities
                .OrderBy(f => DomainConstants.PostCourseSurveyTemplateTitles.Any(title => title.ToLower().Equals(f.Title.ToLower())) ? 0 : 1)
                .ThenBy(f => DomainConstants.PostCourseSurveyTemplateTitles.Any(title => title.ToLower().Equals(f.Title.ToLower())) ? f.Title : string.Empty)
                .ToList();

            foreach (var form in entities)
            {
                if (form.IsStandalone.HasValue && form.IsStandalone.Value)
                {
                    form.CanUnpublishFormStandalone = _formParticipantRepository.Count(m => m.FormOriginalObjectId == form.OriginalObjectId && m.Status == FormParticipantStatus.Completed) == 0;
                }
            }

            return new PagedResultDto<FormModel>(totalCount, entities);
        }
    }
}
