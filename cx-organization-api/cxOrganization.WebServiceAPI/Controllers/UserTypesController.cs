using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.UserTypes;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Authorize]
    public class UserTypesController : ApiControllerBase
    {
        private readonly IUserTypeService _userTypeService;
        private readonly IAdvancedWorkContext _workContext;
        private readonly ISystemRoleService _systemRoleService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userTypeService"></param>
        /// <param name="workContext"></param>
        public UserTypesController(IUserTypeService userTypeService,
            IAdvancedWorkContext workContext,
            ISystemRoleService systemRoleService)
        {
            _userTypeService = userTypeService;
            _workContext = workContext;
            _systemRoleService = systemRoleService;
        }
        /// <summary>
        /// Get Roles
        /// </summary>
        /// <param name="extIds"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        [Route("usertypes")]
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<UserTypeDto>), 200)]
        public IActionResult GetUserTypes([FromQuery] List<ArchetypeEnum> archetypeEnums = null,
            [FromQuery] List<string> extIds = null,
            [FromQuery] List<int> usertypeids = null,
            [FromQuery] bool includeLocalizedData = true,
            [FromQuery] List<int> parentIds = null)
        {
            string langCode = "en-US";
            //if (acceptLanguage == null)
            //    langCode = "en-US";
            //else
            //    langCode = acceptLanguage.FirstOrDefault().Value.Value;

            var roles = _userTypeService.GetUserTypes(extIds: extIds,
                archetypeIds: archetypeEnums,
                userTypeIds: usertypeids,
                ownerId: _workContext.CurrentOwnerId,
                includeLocalizedData: includeLocalizedData,
                langCode: langCode,
                parentIds: parentIds);
            return CreateResponse(roles);
        }


        // LATEST UPDATE USER TYPE API
        [Route("systemroles")]
        [HttpGet]
        [ProducesResponseType(typeof(List<UserTypeDto>), 200)]
        public async Task<IActionResult> GetSystemRolesAsync(
            [FromQuery] List<int> systemRoleIds,
            [FromQuery] List<string> systemRoleExtIds,
            [FromQuery] bool includeLocalizedData,
            [FromQuery] bool includeSystemRolePermissionSubjects
        )
        {
            var token = HttpContext.Request.Headers["Authorization"][0];

            if(token is null)
            {
                return BadRequest();
            }
            var systemRoles = await _systemRoleService.GetSystemRolesConvertedToUserTypesModel(new GetSystemRolesInfoRequest(systemRoleIds,
                                                                                                       systemRoleExtIds,
                                                                                                       includeLocalizedData,
                                                                                                       includeSystemRolePermissionSubjects), token);

            return CreateResponse(systemRoles);
        }
    }
}
