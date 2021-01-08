using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Form.Application.Models;
using Microservice.Form.Common.Helpers;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Form;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Queries
{
    public class SearchFormAnswersQueryHandler : BaseThunderQueryHandler<SearchFormAnswersQuery, PagedResultDto<FormAnswerModel>>
    {
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormUser> _userRepository;
        private readonly IRepository<FormQuestionAnswer> _formQuestionAnswerRepository;
        private readonly IRepository<FormAnswer> _formAnswerRepository;
        private readonly IRepository<FormAnswerAttachment> _formAttachmentRepository;

        public SearchFormAnswersQueryHandler(
            IRepository<FormEntity> formRepository,
            IRepository<FormQuestionAnswer> formQuestionAnswerRepository,
            IRepository<FormAnswer> formAnswerRepository,
            IRepository<FormAnswerAttachment> formAttachmentRepository,
            IRepository<FormUser> userRepository)
        {
            _formRepository = formRepository;
            _formQuestionAnswerRepository = formQuestionAnswerRepository;
            _formAnswerRepository = formAnswerRepository;
            _formAttachmentRepository = formAttachmentRepository;
            _userRepository = userRepository;
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
                    p.CourseId.Equals(query.CourseId)
                    && p.ClassRunId.Equals(query.ClassRunId))
                .WhereIf(
                    query.FormId.HasValue,
                    p => p.FormId.Equals(query.FormId))
                .WhereIf(
                    query.UserId.HasValue,
                    p => p.CreatedBy == query.UserId)
                .WhereIf(
                    query.AssignmentId.HasValue,
                    p => p.AssignmentId.Equals(query.AssignmentId))
                .WhereIf(
                    query.IsSubmitted.HasValue,
                    p => query.IsSubmitted == true ? p.SubmitDate != null : p.SubmitDate == null)
                .WhereIf(
                    query.MyCourseId.HasValue,
                    p => p.MyCourseId == query.MyCourseId)
                .WhereIf(
                    query.IsCompleted.HasValue,
                    p => p.IsCompleted == query.IsCompleted)
                .WhereIf(
                    query.BeforeDueDate.HasValue && form.DueDate.HasValue,
                    p => now < form.DueDate)
                .WhereIf(
                    query.BeforeTimeLimit.HasValue && form.InSecondTimeLimit.HasValue,
                    p => now < p.EndDate);

            formAnswerQuery = ApplySearchByUserAsync(formAnswerQuery, query.SearchText);

            var totalCount = await formAnswerQuery.CountAsync(cancellationToken);

            formAnswerQuery = ApplyPaging(formAnswerQuery, query.PagedInfo);

            var formQuestionAnswersQuery = from fqa in _formQuestionAnswerRepository.GetAll()
                                           join fa in formAnswerQuery on fqa.FormAnswerId equals fa.Id
                                           select fqa;
            var formAnswerAttachments = new List<FormAnswerAttachment>();
            if (form.Type == FormType.Quiz)
            {
                formAnswerAttachments = await _formAttachmentRepository
                    .GetAll()
                    .Where(p => formQuestionAnswersQuery.Any(qa => qa.Id == p.FormQuestionAnswerId))
                    .ToListAsync(cancellationToken);
            }

            var formAnswerAttachmentModels = formAnswerAttachments.Select(x => new FormAnswerAttachmentModel(x));

            var formAnswers = await formAnswerQuery
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync(cancellationToken);

            var formQuestionAnswers = (await formQuestionAnswersQuery.ToListAsync(cancellationToken))
                .Select(p =>
                    new FormQuestionAnswerModel(p, formAnswerAttachmentModels.Where(x => x.FormQuestionAnswerId == p.Id).ToList()));

            var formQuestionAnswersDic = formQuestionAnswers
                .GroupBy(p => p.FormAnswerId, p => p)
                .ToDictionary(p => p.Key, p => p.ToList());

            var entityModels = formAnswers
                .Select(p => new FormAnswerModel(
                    p,
                    formQuestionAnswersDic.ContainsKey(p.Id) ? formQuestionAnswersDic[p.Id] : new List<FormQuestionAnswerModel>()))
                .ToList();
            return new PagedResultDto<FormAnswerModel>(totalCount, entityModels);
        }

        private IQueryable<FormAnswer> ApplySearchByUserAsync(IQueryable<FormAnswer> formAnswerQuery, string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return formAnswerQuery;
            }

            var userQuery = FulltextSearchHelper.BySearchText(_userRepository.GetAll(), searchText, u => u.FullTextSearch);
            return userQuery
                .Join(formAnswerQuery, u => u.Id, a => a.CreatedBy, (user, answer) => answer);
        }
    }
}
