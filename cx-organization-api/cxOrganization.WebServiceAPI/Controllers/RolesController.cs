using System.Collections.Generic;
using System.Net;
using cxOrganization.Client;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Authorize]
    public class RolesController : ApiControllerBase
    {
        private readonly IUserTypeService _userTypeService;
        private readonly IWorkContext _workContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userTypeService"></param>
        /// <param name="workContext"></param>
        public RolesController(IUserTypeService userTypeService,
            IWorkContext workContext)
        {
            _userTypeService = userTypeService;
            _workContext = workContext;
        }
        /// <summary>
        /// Get Roles
        /// </summary>
        /// <param name="extIds"></param>
        /// <param name="roleIds"></param>
        /// <param name="archetypeIds"></param>
        /// <param name="includeLocalizedData"></param>
        /// <returns></returns>
        [Route("roles")]
        [HttpGet]
        [ProducesResponseType(typeof(List<UserTypeDto>), 200)]
        public IActionResult GetRoles(
            [FromQuery] List<string> extIds = null,
            [FromQuery] List<int> roleIds = null,
            [FromQuery] List<ArchetypeEnum> archetypeIds = null,
            bool includeLocalizedData = false)
        {
            var roles = _userTypeService.GetUserTypes(
                extIds: extIds, 
                archetypeIds: archetypeIds is object && archetypeIds.Count > 0
                    ? archetypeIds
                    : new List<ArchetypeEnum> { ArchetypeEnum.Role },
                userTypeIds: roleIds,
                ownerId:_workContext.CurrentOwnerId,
                includeLocalizedData : includeLocalizedData);
            return CreateResponse(roles);
        }
        /// <summary>
        /// Add role to employee
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="employeeMemberDto"></param>
        /// <returns></returns>
        [Route("roles/{roleId}/memberships/employees")]
        [HttpPost]
        [ProducesResponseType(typeof(MemberDto),200)]
        public IActionResult AddRoleToEmployee(int roleId, [FromBody]MemberDto employeeMemberDto)
        {
            var memberDto = _userTypeService.AddUserTypeUser(roleId,
                employeeMemberDto,
                checkingUserArchetypeIds: new List<int> { (int)ArchetypeEnum.Employee });
            return StatusCode((int)HttpStatusCode.Created, memberDto);
        }
        /// <summary>
        /// Remove employee role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="employeeMemberDto"></param>
        /// <returns></returns>
        [Route("roles/{roleId}/memberships/employees")]
        [HttpPut]
        [ProducesResponseType(typeof(MemberDto),200)]
        public IActionResult RemoveEmployeeRole(int roleId, [FromBody]MemberDto employeeMemberDto)
        {
            var memberDto = _userTypeService.RemoveUserTypeUser(roleId,
                employeeMemberDto,
                checkingUserArchetypeIds: new List<int> { (int)ArchetypeEnum.Employee });
            return CreateResponse(memberDto);
        }

    }
}
