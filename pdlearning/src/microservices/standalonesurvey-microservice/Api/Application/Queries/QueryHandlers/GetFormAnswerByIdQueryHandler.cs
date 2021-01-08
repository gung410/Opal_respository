using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormAnswerByIdQueryHandler : BaseThunderQueryHandler<GetSurveyAnswerByIdQuery, SurveyAnswerModel>
    {
        private readonly IRepository<SurveyAnswer> _formAnswerRepository;
        private readonly IRepository<SurveyQuestionAnswer> _formQuestionAnswerRepository;

        public GetFormAnswerByIdQueryHandler(
            IRepository<SurveyAnswer> formAnswerRepository,
            IRepository<SurveyQuestionAnswer> formQuestionAnswerRepository)
        {
            _formAnswerRepository = formAnswerRepository;
            _formQuestionAnswerRepository = formQuestionAnswerRepository;
        }

        protected override async Task<SurveyAnswerModel> HandleAsync(GetSurveyAnswerByIdQuery query, CancellationToken cancellationToken)
        {
            var formAnswer = await _formAnswerRepository
                .GetAll()
                .Where(p => p.Id == query.SurveyAnswerId && p.OwnerId == query.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            var formQuestionAnswers = await _formQuestionAnswerRepository
                .GetAll()
                .Where(p => p.SurveyAnswerId == query.SurveyAnswerId && p.CreatedBy == query.UserId)
                .ToListAsync(cancellationToken);

            if (formAnswer is null)
            {
                throw new SurveyAccessDeniedException();
            }

            return new SurveyAnswerModel(formAnswer, formQuestionAnswers);
        }
    }
}
