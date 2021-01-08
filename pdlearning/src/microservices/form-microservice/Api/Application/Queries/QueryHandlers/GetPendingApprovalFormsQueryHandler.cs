using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Infrastructure;
using Microservice.Form.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Queries
{
    public class GetPendingApprovalFormsQueryHandler : BaseQueryHandler<GetPendingApprovalFormsQuery, PagedResultDto<FormModel>>
    {
        private readonly IRepository<FormEntity> _formRepository;

        public GetPendingApprovalFormsQueryHandler(
            IAccessControlContext accessControlContext,
            IRepository<FormEntity> formRepository) : base(accessControlContext)
        {
            _formRepository = formRepository;
        }

        protected override async Task<PagedResultDto<FormModel>> HandleAsync(GetPendingApprovalFormsQuery query, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository.GetAllWithAccessControl(AccessControlContext, FormEntityExpressions.HasApprovalPermissionExpr(CurrentUserId))
                .Where(p => p.Status == FormStatus.PendingApproval)
                .IgnoreArchivedItems();

            var totalCount = await formQuery.CountAsync(cancellationToken);

            formQuery = formQuery.OrderByDescending(p => p.ChangedDate != null ? p.ChangedDate : p.CreatedDate);

            formQuery = ApplyPaging(formQuery, query.PagedInfo);

            var entities = await formQuery
                .Select(p => new FormModel(p))
                .ToListAsync(cancellationToken);

            return new PagedResultDto<FormModel>(totalCount, entities);
        }
    }
}
