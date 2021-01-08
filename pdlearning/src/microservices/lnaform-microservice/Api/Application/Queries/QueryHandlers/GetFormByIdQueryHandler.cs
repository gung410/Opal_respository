using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Common.Extensions;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormByIdQueryHandler : BaseQueryHandler<GetFormByIdQuery, FormModel>
    {
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;

        public GetFormByIdQueryHandler(
            IRepository<AccessRight> accessRightRepository,
            IAccessControlContext accessControlContext,
            IRepository<FormEntity> formRepository) : base(accessControlContext)
        {
            _formRepository = formRepository;
            _accessRightRepository = accessRightRepository;
        }

        protected override async Task<FormModel> HandleAsync(GetFormByIdQuery query, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository
                .GetAll()
                .ApplyAccessControlEx(
                    AccessControlContext,
                    LnaFormEntityExpressions.HasPermissionToSeeFormExpr(CurrentUserId))
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId);

            var form = await formQuery.FirstOrDefaultAsync(p => p.Id == query.FormId, cancellationToken);

            if (form is null)
            {
                throw new FormAccessDeniedException();
            }

            return new FormModel(form);
        }
    }
}
