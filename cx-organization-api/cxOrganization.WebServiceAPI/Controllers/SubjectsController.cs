using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace cxOrganization.WebServiceAPI.Controllers
{
    /// <summary>
    /// Subjects controller
    /// </summary>
    [Authorize]
    public class SubjectsController : ApiControllerBase
    {
        private readonly IUserGroupService _userGroupService;

        /// <summary>
        /// Setup controller
        /// </summary>
        /// <param name="userGroupService"></param>
        public SubjectsController(IUserGroupService userGroupService,
            IAdvancedWorkContext workContext,
            IUserGroupTypeService userGroupTypeService,
            IUserService userService,
            IDepartmentService departmentService)
        {
            _userGroupService = userGroupService;
        }

        /// <summary>
        /// Get list of teaching subject
        /// </summary>
        /// <param name="archetypeIds"></param>
        /// <returns></returns>
        [Route("subjects")]
        [HttpGet]
        [ProducesResponseType(typeof(List<TeachingSubjectDto>), 200)]
        public IActionResult GetUserGroups(List<ArchetypeEnum> archetypeIds)
        {
            var result = _userGroupService.GetUserGroupsByArchetypes(archetypeIds);
            return CreateResponse(result);
        }
    }
}
