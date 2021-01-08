using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormSectionsByFormIdQueryHandler : BaseQueryHandler<GetFormSectionsByFormIdQuery, List<SurveySectionModel>>
    {
        private readonly IRepository<SurveySection> _formSectionRepository;

        public GetFormSectionsByFormIdQueryHandler(
            IRepository<SurveySection> formSectionRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _formSectionRepository = formSectionRepository;
        }

        protected override async Task<List<SurveySectionModel>> HandleAsync(GetFormSectionsByFormIdQuery query, CancellationToken cancellationToken)
        {
            var formSections = await this._formSectionRepository
                .GetAll()
                .Where(formSection => formSection.SurveyId == query.FormId)
                .ToListAsync(cancellationToken);

            if (formSections == null)
            {
                throw new EntityNotFoundException(typeof(SurveySection), query.FormId);
            }

            return formSections.Select(formSection => new SurveySectionModel(formSection)).ToList();
        }
    }
}
