using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Commands;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.Queries;
using Microservice.Form.Application.RequestDtos;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Services
{
    public class QuestionBankService : ApplicationService, IQuestionBankService
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public QuestionBankService(
            IUnitOfWorkManager unitOfWorkManager,
            IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<QuestionBankModel> CreateQuestionBank(SaveQuestionBankRequest request, Guid userId)
        {
            var command = request.ToCommand(userId, true);
            await _thunderCqrs.SendCommand(command);
            return await _thunderCqrs.SendQuery(new GetQuestionBankByIdQuery { QuestionBankId = command.Id });
        }

        public async Task<QuestionBankModel> UpdateQuestionBank(SaveQuestionBankRequest request, Guid userId)
        {
            var command = request.ToCommand(userId);
            await _thunderCqrs.SendCommand(command);
            return await _thunderCqrs.SendQuery(new GetQuestionBankByIdQuery { QuestionBankId = command.Id });
        }

        public async Task DeleteQuestionBank(Guid id)
        {
            await _thunderCqrs.SendCommand(new DeleteQuestionBankCommand
            {
                QuestionBankId = id
            });
        }

        public async Task<PagedResultDto<QuestionBankModel>> SearchQuestionBank(SearchQuestionBankRequest request)
        {
            var query = request.ToQuery();
            return await _thunderCqrs.SendQuery(query);
        }

        public async Task<PagedResultDto<QuestionGroupModel>> SearchQuestionGroup(SearchQuestionGroupRequest request)
        {
            var query = request.ToQuery();
            return await _thunderCqrs.SendQuery(query);
        }
    }
}
