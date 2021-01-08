using System.Threading.Tasks;
using Microservice.Uploader.Application.Models;
using Microservice.Uploader.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Controllers
{
    [Route("api/personal-spaces")]
    public class PersonalSpaceController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IPersonalSpaceApplicationService _personalSpaceService;

        public PersonalSpaceController(IThunderCqrs thunderCqrs, IUserContext userContext, IPersonalSpaceApplicationService formApplicationService) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
            _personalSpaceService = formApplicationService;
        }

        // TODO: RENAME THIS API TO 'myPersonalSpace'
        [HttpGet("")]
        public async Task<PersonalSpaceModel> GetPersonalSpace()
        {
            return await _personalSpaceService.GetPersonalSpaceByUserId(CurrentUserId);
        }
    }
}
