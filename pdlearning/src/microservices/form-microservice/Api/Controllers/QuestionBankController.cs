using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Controllers
{
    [Route("api/question-bank")]
    public class QuestionBankController : ApplicationApiController
    {
        private readonly IQuestionBankService _questionBankService;

        public QuestionBankController(IUserContext userContext, IQuestionBankService questionBankService) : base(userContext)
        {
            _questionBankService = questionBankService;
        }

        [HttpPost("create")]
        public async Task<QuestionBankModel> CreateQuestionBank([FromBody] SaveQuestionBankRequest dto)
        {
            return await _questionBankService.CreateQuestionBank(dto, CurrentUserId);
        }

        [HttpPut("update")]
        public async Task<QuestionBankModel> UpdateQuestionBank([FromBody] SaveQuestionBankRequest dto)
        {
            return await _questionBankService.UpdateQuestionBank(dto, CurrentUserId);
        }

        [HttpDelete("{questionBankId:guid}")]
        public async Task DeleteQuestionBank(Guid questionBankId)
        {
            await _questionBankService.DeleteQuestionBank(questionBankId);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<QuestionBankModel>> SearchQuestionBanks([FromBody] SearchQuestionBankRequest dto)
        {
            return await _questionBankService.SearchQuestionBank(dto);
        }

        [HttpPost("question-group/search")]
        public async Task<PagedResultDto<QuestionGroupModel>> SearchQuestionGroup([FromBody] SearchQuestionGroupRequest dto)
        {
            return await _questionBankService.SearchQuestionGroup(dto);
        }
    }
}
