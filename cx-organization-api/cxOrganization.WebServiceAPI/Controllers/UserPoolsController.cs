using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Validators;
using cxOrganization.WebServiceAPI.ActionFilters;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Security.AccessServices;
using Microsoft.Extensions.Logging;

namespace cxOrganization.WebServiceAPI.Controllers
{
    /// <summary>
    /// UserPool API  which will be used for general purpose of grouping users.
    /// </summary>
    [Authorize]
    public class UserPoolsController : ApiControllerBase
    {
        private readonly IWorkContext _workContext;
        private readonly IUserGroupService _userGroupService;
        private readonly IUserPoolMemberService _userPoolMemberService;
        private readonly IUserPoolAccessService _userPoolAccessService;
        private readonly ILogger _logger;
        public UserPoolsController(ILogger<UserPoolsController> logger, 
            IWorkContext workContext,
            Func<ArchetypeEnum, IUserGroupService> userGroupService,
            IUserPoolMemberService userPoolMemberService,
            IUserPoolAccessService userPoolAccessService)
        {
            _logger = logger;
            _workContext = workContext;
            _userGroupService = userGroupService(ArchetypeEnum.UserPool);
            _userPoolMemberService = userPoolMemberService;
            _userPoolAccessService = userPoolAccessService;
        }

        /// <summary>
        /// Gets the list of user pools.
        /// </summary>
        /// <param name="userPoolIds"></param>
        /// <param name="departmentIds"></param>
        /// <param name="poolOwnerUserIds"></param>
        /// <param name="memberUserIds"></param>
        /// <param name="statusEnums"></param>
        /// <param name="groupTypes"></param>
        /// <param name="extIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="countActiveMembers"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        [Route("userpools")]
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<UserPoolDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public  async Task<IActionResult> GetList(
            [FromQuery] List<int> userPoolIds = null,
            [FromQuery] List<int> departmentIds = null,
            [FromQuery] List<int> poolOwnerUserIds = null,
            [FromQuery] List<int> memberUserIds = null,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            [FromQuery] List<GrouptypeEnum> groupTypes = null,
            [FromQuery] List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? countActiveMembers = false,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            if (!ValidateMinimalFilter(userPoolIds, departmentIds, poolOwnerUserIds, memberUserIds, extIds))
            {
                return NoContent();
            }

            var userGroupAccessChecking = await 
                _userPoolAccessService.CheckReadUserPoolAccess(_workContext, poolOwnerUserIds, departmentIds);
            if (!userGroupAccessChecking.AccessStatus.IsAllowedAccess())
                return NoContent();

            poolOwnerUserIds = userGroupAccessChecking.UserIds;
            departmentIds = userGroupAccessChecking.ParentDepartmentIds;

            var pagingUserPoolDto = _userGroupService.GetUserGroups<UserPoolDto>(
                departmentIds: departmentIds,
                groupUserIds: poolOwnerUserIds,
                userGroupIds: userPoolIds,
                memberUserIds: memberUserIds,
                statusIds: statusEnums,
                archetypeIds: new List<int>() { (int)ArchetypeEnum.UserPool },
                extIds: extIds,
                groupTypes: groupTypes,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy
                );

            // Count active members
            if(countActiveMembers == true && pagingUserPoolDto.Items.Any())
            {
                var existingUserPoolIds = pagingUserPoolDto
                        .Items
                        .Where(p=> p.Identity?.Id > 0)
                        .Select(p => (int)p.Identity.Id).ToList();
                var countingMembers = _userPoolMemberService.CountMemberGroupByUserPools(existingUserPoolIds);
                foreach (var userPool in pagingUserPoolDto.Items)
                {
                    var userPoolId = (int)userPool.Identity.Id;
                    if (countingMembers.ContainsKey(userPoolId))
                    {
                        userPool.MemberCount = countingMembers[userPoolId];
                    }
                }
            }

            return CreateResponse(pagingUserPoolDto);
        }

        private bool ValidateMinimalFilter(List<int> userPoolIds, List<int> departmentIds, List<int> poolOwnerUserIds, List<int> memberUserIds,
            List<string> extIds)
        {
            //Only validate when authorized by user token
            if (!string.IsNullOrEmpty(_workContext.Sub))
            {
                if (userPoolIds.IsNullOrEmpty()
                    && departmentIds.IsNullOrEmpty()
                    && poolOwnerUserIds.IsNullOrEmpty()
                    && memberUserIds.IsNullOrEmpty()
                    && extIds.IsNullOrEmpty())
                {
                    _logger.LogWarning(
                        "For security reason, it requires minimal filter on identity of user, department or user group to be able to retrieve user groups.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a user pools by identifier.
        /// </summary>
        /// <param name="userPoolId"></param>
        /// <param name="statusEnums"></param>
        /// <returns></returns>
        [Route("userpools/{userPoolId}")]
        [HttpGet]
        [ProducesResponseType(typeof(UserPoolDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(int userPoolId, [FromQuery] List<EntityStatusEnum> statusEnums = null)
        {
            var userPool = _userGroupService.GetUserGroups<UserPoolDto>(
                userGroupIds: new List<int>() { userPoolId },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.UserPool },
                statusIds: statusEnums).Items.FirstOrDefault();

            return CreateResponse(userPool);
        }

        /// <summary>
        /// Inserts a new user pools.
        /// </summary>
        /// <returns></returns>
        [Route("userpools")]
        [HttpPost]
        [ProducesResponseType(typeof(UserPoolDto), StatusCodes.Status201Created)]
        [TypeFilter(typeof(PreventXSSFilter))]
        public IActionResult Insert([Required] [FromBody]UserPoolDto userPoolDto)
        {
            var validationSpecification = userPoolDto.GetParentDepartmentId() > 0
                ? (new HierarchyDepartmentValidationBuilder())
                        .ValidateDepartment((int)userPoolDto.GetParentDepartmentId(), ArchetypeEnum.Unknown)
                        .SkipCheckingArchetype()
                        .WithStatus(EntityStatusEnum.All)
                        .IsDirectParent()
                        .Create()
                : null;

            userPoolDto.Identity.Id = 0;
            var group = _userGroupService.InsertUserGroup(validationSpecification, userPoolDto);

            return StatusCode(StatusCodes.Status201Created, group);
        }

        /// <summary>
        /// Updates the existing user pools.
        /// </summary>
        /// <param name="userPoolId"></param>
        /// <param name="userPoolDto"></param>
        /// <returns></returns>
        [Route("userpools/{userPoolId}")]
        [HttpPut]
        [ProducesResponseType(typeof(UserPoolDto), StatusCodes.Status200OK)]
        [TypeFilter(typeof(PreventXSSFilter))]
        public IActionResult Update(int userPoolId, [Required] [FromBody]UserPoolDto userPoolDto, [FromQuery]bool countActiveMembers = false)
        {
            var validationSpecification = userPoolDto.GetParentDepartmentId() > 0
                ? (new HierarchyDepartmentValidationBuilder())
                        .ValidateDepartment((int)userPoolDto.GetParentDepartmentId(), ArchetypeEnum.Unknown)
                        .SkipCheckingArchetype()
                        .WithStatus(EntityStatusEnum.All)
                        .IsDirectParent()
                        .Create()
                : null;

            userPoolDto.Identity.Id = userPoolId;
            var group = _userGroupService.UpdateUserGroup(validationSpecification, userPoolDto);

            // Count active members
            if (countActiveMembers && group is object)
            {
                var existingUserPoolId = Convert.ToInt32((group as UserPoolDto).Identity.Id.Value);
                var countingMembers = _userPoolMemberService.CountMemberGroupByUserPools(new List<int> { existingUserPoolId });
                var currentUserPoolId = (int)group.Identity.Id;
                if (countingMembers.ContainsKey(currentUserPoolId))
                {
                    (group as UserPoolDto).MemberCount = countingMembers[currentUserPoolId];
                }
            }

            return CreateResponse(group);
        }

        /// <summary>
        /// Removes the existing user pools.
        /// </summary>
        /// <param name="userPoolId"></param>
        /// <returns></returns>
        [Route("userpools/{userPoolId}")]
        [HttpDelete]
        public IActionResult Remove(int userPoolId)
        {
            var userPool = _userGroupService.GetUserGroups<UserPoolDto>(
                userGroupIds: new List<int>() { userPoolId },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.UserPool },
                statusIds: new List<EntityStatusEnum>() { EntityStatusEnum.Active }).Items.FirstOrDefault();

            if (userPool == null) return CreateNotFoundResponse<UserPoolDto>();

            userPool.EntityStatus.StatusId = EntityStatusEnum.Deactive;
            userPool.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            userPool.EntityStatus.LastUpdated = DateTime.Now;

            var group = _userGroupService.UpdateUserGroup(null, userPool);

            return CreateResponse(group);
        }

        /// <summary>
        /// Gets a list members of the user pool.
        /// </summary>
        /// <param name="userPoolId"></param>
        /// <returns></returns>
        [Route("userpools/{userPoolId}/members")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MembershipDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetUserMembers(int userPoolId)
        {
            var members = _userPoolMemberService.GetMembers(userPoolId);

            return CreateResponse(members);
        }

        /// <summary>
        /// Adds new members into the user pool.
        /// </summary>
        /// <param name="userPoolId"></param>
        /// <param name="membershipDto"></param>
        /// <returns></returns>
        [Route("userpools/{userPoolId}/members")]
        [HttpPost]
        [ProducesResponseType(typeof(List<MembershipDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddMembers(int userPoolId, [Required] [FromBody] List<MembershipDto> membershipDtos)
        {
            var members = _userPoolMemberService.AddMembers(userPoolId, membershipDtos);
            if(members == null || !members.Any())
            {
                return CreateNoContentResponse();
            }
            return StatusCode(StatusCodes.Status201Created, members);
        }

        /// <summary>
        /// Removes members from the existing user pool.
        /// </summary>
        /// <param name="userPoolId"></param>
        /// <param name="membershipDto"></param>
        /// <returns></returns>
        [Route("userpools/{userPoolId}/members")]
        [HttpDelete]
        [ProducesResponseType(typeof(List<MembershipDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult RemoveMembers(int userPoolId, [Required] [FromBody] List<MembershipDto> membershipDtos)
        {
            var members = _userPoolMemberService.RemoveMembers(userPoolId, membershipDtos);
            return CreateResponse(members);
        }
    }
}