using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.Entities;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Form.Application.Queries
{
    public class GetQuestionBankByIdQueryHandler : BaseQueryHandler<GetQuestionBankByIdQuery, QuestionBankModel>
    {
        private readonly IRepository<QuestionGroup> _questionGroupRepository;
        private readonly IRepository<QuestionBank> _questionBankRepository;

        public GetQuestionBankByIdQueryHandler(
        IRepository<QuestionGroup> questionGroupRepository,
        IRepository<QuestionBank> questionBankRepository,
        IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _questionGroupRepository = questionGroupRepository;
            _questionBankRepository = questionBankRepository;
        }

        protected override async Task<QuestionBankModel> HandleAsync(GetQuestionBankByIdQuery query, CancellationToken cancellationToken)
        {
            var questionBank = await _questionBankRepository.FirstOrDefaultAsync(p => p.Id == query.QuestionBankId && p.CreatedBy == CurrentUserId);

            if (questionBank is null)
            {
                throw new EntityNotFoundException($"{nameof(questionBank)} not found");
            }

            var result = new QuestionBankModel(questionBank);

            if (questionBank.QuestionGroupId.HasValue)
            {
                var questionGroup = await _questionGroupRepository.FirstOrDefaultAsync(questionBank.QuestionGroupId.Value);
                result.QuestionGroupName = questionGroup != null ? questionGroup.Name : string.Empty;
            }

            return result;
        }
    }
}
