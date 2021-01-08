using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Form.Application.Services
{
    public interface IQuestionBankService
    {
        Task<QuestionBankModel> CreateQuestionBank(SaveQuestionBankRequest request, Guid userId);

        Task<QuestionBankModel> UpdateQuestionBank(SaveQuestionBankRequest request, Guid userId);

        Task DeleteQuestionBank(Guid id);

        Task<PagedResultDto<QuestionBankModel>> SearchQuestionBank(SearchQuestionBankRequest request);

        Task<PagedResultDto<QuestionGroupModel>> SearchQuestionGroup(SearchQuestionGroupRequest request);
    }
}
