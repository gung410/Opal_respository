using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Common.Extensions;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Microservice.LnaForm.Infrastructure;
using Microservice.LnaForm.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Queries
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
            var unionExpression = LnaFormEntityExpressions.HasOwnerPermissionExpr(CurrentUserId);

            if (query.IncludeFormForImportToCourse)
            {
                unionExpression = LnaFormEntityExpressions.AvailableForImportToCourseExpr(CurrentUserId);
            }

            var formQuery = _formRepository
                .GetAll()
                .ApplyAccessControlEx(AccessControlContext, unionExpression)
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                .IgnoreArchivedItems();

            formQuery = formQuery.WhereIf(
                !string.IsNullOrEmpty(query.SearchFormTitle),
                f => EF.Functions.Like(f.Title, $"%{query.SearchFormTitle}%"));

            formQuery = formQuery.WhereIf(
                !query.FilterByStatus.IsNullOrEmpty(),
                f => query.FilterByStatus.Contains(f.Status));

            if (query.FilterByStatus.IsNullOrEmpty())
            {
                formQuery = formQuery.Where(r => r.Status != FormStatus.Archived);
            }

            var totalCount = await formQuery.CountAsync(cancellationToken);

            formQuery = formQuery.OrderByDescending(p => p.ChangedDate != null ? p.ChangedDate.Value : p.CreatedDate);

            formQuery = ApplyPaging(formQuery, query.PagedInfo);

            var entities = await formQuery.Select(p => new FormModel(p)).ToListAsync(cancellationToken);
            foreach (var form in entities)
            {
                form.CanUnpublishFormStandalone = _formParticipantRepository.Count(m => m.FormOriginalObjectId == form.OriginalObjectId && m.Status == FormParticipantStatus.Completed) == 0;
            }

            return new PagedResultDto<FormModel>(totalCount, entities);
        }
    }
}
