using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Domain.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormSectionByIdQueryHandler : BaseQueryHandler<GetFormSectionByIdQuery, FormSectionModel>
    {
        private readonly IRepository<FormSection> _formSectionRepository;

        public GetFormSectionByIdQueryHandler(
            IRepository<FormSection> formSectionRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _formSectionRepository = formSectionRepository;
        }

        protected override async Task<FormSectionModel> HandleAsync(GetFormSectionByIdQuery query, CancellationToken cancellationToken)
        {
            var formSection = await this._formSectionRepository.GetAsync(query.Id);

            if (formSection == null)
            {
                throw new EntityNotFoundException(typeof(FormSection), query.Id);
            }

            return new FormSectionModel(formSection);
        }
    }
}
