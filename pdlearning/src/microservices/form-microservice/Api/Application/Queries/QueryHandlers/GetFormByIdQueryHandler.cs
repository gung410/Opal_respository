using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Models;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Queries
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
            var formQuery = _formRepository.GetAll().Where(p => p.Id == query.FormId);

            if (!(await formQuery.AnyAsync(cancellationToken)))
            {
                throw new FormNotAvailableException();
            }

            formQuery = formQuery
                .ApplyAccessControl(AccessControlContext, FormEntityExpressions.HasPermissionToSeeFormExpr(CurrentUserId))
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                .CombineWithPublicSurveyTemplates(_formRepository);

            var form = await formQuery.FirstOrDefaultAsync(cancellationToken);

            if (form is null)
            {
                throw new FormAccessDeniedException();
            }

            return new FormModel(form);
        }
    }
}
