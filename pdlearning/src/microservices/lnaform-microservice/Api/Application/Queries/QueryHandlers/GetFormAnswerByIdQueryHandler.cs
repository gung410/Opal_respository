using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormAnswerByIdQueryHandler : BaseThunderQueryHandler<GetFormAnswerByIdQuery, FormAnswerModel>
    {
        private readonly IRepository<FormAnswer> _formAnswerRepository;
        private readonly IRepository<FormQuestionAnswer> _formQuestionAnswerRepository;

        public GetFormAnswerByIdQueryHandler(
            IRepository<FormAnswer> formAnswerRepository,
            IRepository<FormQuestionAnswer> formQuestionAnswerRepository)
        {
            _formAnswerRepository = formAnswerRepository;
            _formQuestionAnswerRepository = formQuestionAnswerRepository;
        }

        protected override async Task<FormAnswerModel> HandleAsync(GetFormAnswerByIdQuery query, CancellationToken cancellationToken)
        {
            var formAnswer = await _formAnswerRepository
                .GetAll()
                .Where(p => p.Id == query.FormAnswerId && p.OwnerId == query.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            var formQuestionAnswers = await _formQuestionAnswerRepository
                .GetAll()
                .Where(p => p.FormAnswerId == query.FormAnswerId && p.CreatedBy == query.UserId)
                .ToListAsync(cancellationToken);

            if (formAnswer is null)
            {
                throw new FormAccessDeniedException();
            }

            return new FormAnswerModel(formAnswer, formQuestionAnswers);
        }
    }
}
