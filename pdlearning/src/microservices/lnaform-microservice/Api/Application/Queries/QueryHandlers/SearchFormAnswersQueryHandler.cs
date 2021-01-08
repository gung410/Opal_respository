using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Queries
{
    public class SearchFormAnswersQueryHandler : BaseThunderQueryHandler<SearchFormAnswersQuery, PagedResultDto<FormAnswerModel>>
    {
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormQuestionAnswer> _formQuestionAnswerRepository;
        private readonly IRepository<FormAnswer> _formAnswerRepository;

        public SearchFormAnswersQueryHandler(
            IRepository<FormEntity> formRepository,
            IRepository<FormQuestionAnswer> formQuestionAnswerRepository,
            IRepository<FormAnswer> formAnswerRepository)
        {
            _formRepository = formRepository;
            _formQuestionAnswerRepository = formQuestionAnswerRepository;
            _formAnswerRepository = formAnswerRepository;
        }

        protected override async Task<PagedResultDto<FormAnswerModel>> HandleAsync(SearchFormAnswersQuery query, CancellationToken cancellationToken)
        {
            var form = await _formRepository.FirstOrDefaultAsync(p => p.Id.Equals(query.FormId));

            if (form == null)
            {
                throw new FormAccessDeniedException();
            }

            var now = Clock.Now;
            var formAnswerQuery = _formAnswerRepository.GetAll()
                .Where(p =>
                    p.FormId.Equals(query.FormId)
                    && p.ResourceId.Equals(query.ResourceId)
                    && p.OwnerId == query.UserId)
                .WhereIf(
                    query.IsSubmitted.HasValue,
                    p => query.IsSubmitted == true ? p.SubmitDate != null : p.SubmitDate == null)
                .WhereIf(
                    query.IsCompleted.HasValue,
                    p => p.IsCompleted == query.IsCompleted)
                .WhereIf(
                    query.BeforeDueDate.HasValue && form.DueDate.HasValue,
                    p => now < form.DueDate);

            var totalCount = await formAnswerQuery.CountAsync(cancellationToken);

            formAnswerQuery = ApplyPaging(formAnswerQuery, query.PagedInfo);

            var formQuestionAnswersQuery = from fqa in _formQuestionAnswerRepository.GetAll()
                                           join fa in formAnswerQuery on fqa.FormAnswerId equals fa.Id
                                           select fqa;

            var formAnswers = await formAnswerQuery
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync(cancellationToken);

            var formQuestionAnswersDic = (await formQuestionAnswersQuery.ToListAsync(cancellationToken))
                .GroupBy(p => p.FormAnswerId, p => p)
                .ToDictionary(p => p.Key, p => p.ToList());

            var entityModels = formAnswers
                .Select(p => new FormAnswerModel(
                    p,
                    formQuestionAnswersDic.ContainsKey(p.Id) ? formQuestionAnswersDic[p.Id] : new List<FormQuestionAnswer>()))
                .ToList();

            return new PagedResultDto<FormAnswerModel>(totalCount, entityModels);
        }
    }
}
