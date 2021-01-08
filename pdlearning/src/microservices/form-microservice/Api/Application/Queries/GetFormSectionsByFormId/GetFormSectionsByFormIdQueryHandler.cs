using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Form.Application.Queries
{
    public class GetFormSectionsByFormIdQueryHandler : BaseQueryHandler<GetFormSectionsByFormIdQuery, List<FormSectionModel>>
    {
        private readonly IRepository<FormSection> _formSectionRepository;

        public GetFormSectionsByFormIdQueryHandler(
            IRepository<FormSection> formSectionRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _formSectionRepository = formSectionRepository;
        }

        protected override async Task<List<FormSectionModel>> HandleAsync(GetFormSectionsByFormIdQuery query, CancellationToken cancellationToken)
        {
            var formSections = await this._formSectionRepository
                .GetAll()
                .Where(formSection => formSection.FormId == query.FormId)
                .ToListAsync(cancellationToken);

            if (formSections == null)
            {
                throw new EntityNotFoundException(typeof(FormSection), query.FormId);
            }

            return formSections.Select(formSection => new FormSectionModel(formSection)).ToList();
        }
    }
}
