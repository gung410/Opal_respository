using System;
using System.Threading.Tasks;
using Microservice.Uploader.Application.Models;
using Microservice.Uploader.Application.RequestDtos;
using Microservice.Uploader.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Controllers
{
    [Route("api/personal-files")]
    public class PersonalFileController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IPersonalSpaceApplicationService _personalSpaceService;

        public PersonalFileController(IThunderCqrs thunderCqrs, IUserContext userContext, IPersonalSpaceApplicationService formApplicationService) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
            _personalSpaceService = formApplicationService;
        }

        [HttpGet("{fileId:guid}")]
        public async Task<PersonalFileModel> GetPersonalFile(Guid fileId)
        {
            return await _personalSpaceService.GetPersonalFileById(fileId, CurrentUserId);
        }

        [HttpPost("create")]
        public Task CreatePersonalFile([FromBody] CreatePersonalFilesRequest request)
        {
            return _personalSpaceService.CreatePersonalFile(request);
        }

        [HttpDelete("{fileId:guid}")]
        public async Task DeletePersonalFile(Guid fileId)
        {
            await _personalSpaceService.DeletePersonalFile(fileId, CurrentUserId);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<PersonalFileModel>> SearchPersonalFiles([FromBody] SearchPersonalFilesRequestDto dto)
        {
            return await _personalSpaceService.SearchPersonalFiles(dto, CurrentUserId);
        }
    }
}
