using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Domain.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormSectionByIdQueryHandler : BaseQueryHandler<GetFormSectionByIdQuery, SurveySectionModel>
    {
        private readonly IRepository<SurveySection> _formSectionRepository;

        public GetFormSectionByIdQueryHandler(
            IRepository<SurveySection> formSectionRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _formSectionRepository = formSectionRepository;
        }

        protected override async Task<SurveySectionModel> HandleAsync(GetFormSectionByIdQuery query, CancellationToken cancellationToken)
        {
            var formSection = await this._formSectionRepository.GetAsync(query.Id);

            if (formSection == null)
            {
                throw new EntityNotFoundException(typeof(SurveySection), query.Id);
            }

            return new SurveySectionModel(formSection);
        }
    }
}
