using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cxOrganization.WebServiceAPI.Controllers
{
    /// <summary>
    /// UserGroups controller
    /// </summary>
    [Authorize]
    public class UserGroupsController : ApiControllerBase
    {
        private readonly IUserGroupService _userGroupService;
        private readonly IWorkContext _workContext;
        private readonly IUserGroupTypeService _userGroupTypeService;
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;
        /// <summary>
        /// Setup controller
        /// </summary>
        /// <param name="userGroupService"></param>
        /// <param name="workContext"></param>
        /// <param name="departmentService"></param>
        /// <param name="userGroupTypeService"></param>
        /// <param name="userService"></param>
        public UserGroupsController(IUserGroupService userGroupService,
            IWorkContext workContext,
            IUserGroupTypeService userGroupTypeService,
            IUserService userService,
            IDepartmentService departmentService)
        {
            _workContext = workContext;
            _userGroupTypeService = userGroupTypeService;
            _userGroupService = userGroupService;
            _userService = userService;
            _departmentService = departmentService;
        }

        /// <summary>
        /// Ge tUserGroups Without Filtering CxToken
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="customerIds"></param>
        /// <param name="departmentIds"></param>
        /// <param name="userGroupStatusEnums"></param>
        /// <param name="userGroupArchetypeIds"></param>
        /// <param name="userIds"></param>
        /// <param name="userGroupIds"></param>
        /// <param name="extIds"></param>
        /// <param name="groupTypes"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/usergroups")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>),200)]
        public IActionResult GetUserGroupsWithoutFilterCxToken(int ownerId,
            [FromQuery] List<int> customerIds,
            [FromQuery] List<int> departmentIds,
            [FromQuery] List<EntityStatusEnum> userGroupStatusEnums,
            [FromQuery] List<int> userGroupArchetypeIds,
            [FromQuery] List<int> userIds,
            [FromQuery] List<int> userGroupIds,
            [FromQuery] List<string> extIds = null,
            [FromQuery] List<GrouptypeEnum> groupTypes = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var pagingDto = _userGroupService.GetUserGroups<IdentityStatusDto>(ownerId: ownerId,
                memberUserIds: userIds,
                userGroupIds: userGroupIds,
                customerIds: customerIds,
                departmentIds: departmentIds,
                statusIds: userGroupStatusEnums,
                groupTypes : groupTypes,
                archetypeIds: userGroupArchetypeIds,
                extIds: extIds,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items, pagingDto.PageIndex, pagingDto.PageSize, pagingDto.HasMoreData);
        }

        /// <summary>
        /// Get UserGroup Without Filtering CxToken
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/usergroups/{usergroupId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IdentityStatusDto),200)]
        public IActionResult GetUserGroupWithoutFilterCxToken(int ownerId, int userGroupId)
        {
            var userGroupDto = _userGroupService.GetUserGroups<IdentityStatusDto>(ownerId: ownerId,
                userGroupIds: new List<int> { userGroupId },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            return CreateResponse<IdentityStatusDto>(userGroupDto);
        }

        /// <summary>
        /// Get user group by user group and owner id without filtering CXToken
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/usergroups/{usergroupId}/usergrouptypes")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>),200)]
        public IActionResult GetUserGroupTypeUGWithoutFilterCxToken(int ownerId, int userGroupId)
        {
            var userGroupTypeIdentityStatusDtos = _userGroupTypeService.GetUserGroupTypes(ownerId: ownerId,
                userGroupIds: new List<int> { userGroupId });
            return CreateResponse(userGroupTypeIdentityStatusDtos);
        }


        /// <summary>
        /// Get Users In UserGroup Without Filtering CxToken
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userGroupId"></param>
        /// <param name="customerIds"></param>
        /// <param name="parentDepartmentIds"></param>
        /// <param name="userStatusEnums"></param>
        /// <param name="userArchetypeIds"></param>
        /// <param name="userIds"></param>
        /// <param name="ssnList"></param>
        /// <param name="extIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userNames"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/usergroups/{usergroupId}/users")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>),200)]
        public IActionResult GetUsersInUserGroupWithoutFilterCxToken(int ownerId,
            int userGroupId,
            [FromQuery] List<int> customerIds,
            [FromQuery] List<int> parentDepartmentIds,
            [FromQuery] List<EntityStatusEnum> userStatusEnums,
            [FromQuery] List<ArchetypeEnum> userArchetypeIds,
            [FromQuery] List<int> userIds,
            [FromQuery] List<string> ssnList = null,
            [FromQuery] List<string> extIds = null,
            [FromQuery] List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var pagingDto = _userService.GetUsers<IdentityStatusDto>(ownerId: ownerId,
                userGroupIds: new List<int> { userGroupId },
                customerIds: customerIds,
                parentDepartmentIds: parentDepartmentIds,
                statusIds: userStatusEnums,
                archetypeIds: userArchetypeIds,
                userIds: userIds,
                ssnList: ssnList,
                extIds: extIds,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                userNames : userNames,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items, pagingDto.PageIndex, pagingDto.PageSize, pagingDto.HasMoreData);
        }
        /// <summary>
        /// Get user in usergroup without filtering CxToken
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userGroupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/usergroups/{usergroupId}/users/{userId}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>),200)]
        public IActionResult GetUserInUserGroupWithoutFilterCxToken(int ownerId,
            int userGroupId,
            int userId)
        {
            var userIdentityStatusDto = _userService.GetUsers<IdentityStatusDto>(ownerId: ownerId,
                userIds: new List<int> { userId },
                userGroupIds: new List<int> { userGroupId }).Items.FirstOrDefault();
            return CreateResponse<IdentityStatusDto>(userIdentityStatusDto);
        }

        /// <summary>
        /// Get user group department without filtering Cxtoken
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/usergroups/{usergroupId}/departments")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>),200)]
        public IActionResult GetUserGroupDepartmentWithoutFilterCxToken(int ownerId,
            int userGroupId)
        {
            var departmentIdentityStatusDto = _departmentService.GetDepartments<IdentityStatusDto>(ownerId: ownerId,
                userGroupIds: new List<int> { userGroupId }).Items;
            return departmentIdentityStatusDto.Any() ? CreateResponse(departmentIdentityStatusDto) : CreateNotFoundResponse<IdentityStatusDto>();
        }
        /// <summary>
        /// Get usergroup by extid
        /// </summary>
        /// <param name="usergroupextid"></param>
        /// <returns></returns>
        [Route("usergroups/extid/{usergroupextid}/identity")]
        [HttpGet]
        [ProducesResponseType(typeof(IdentityStatusDto),200)]
        public IActionResult GetUserIdentityStatusByExtId(string usergroupextid)
        {
            var userGroupDto = _userGroupService.GetUserGroupIdentityStatusByExtId(usergroupextid, _workContext.CurrentCustomerId);
            if (userGroupDto == null)
            {
                return CreateNotFoundResponse(string.Format("UserGroup not found: UserGroupExtId({0})", usergroupextid));
            }
            return CreateResponse(userGroupDto);
        }

        /// <summary>
        /// Get list user group by extid
        /// </summary>
        /// <param name="usergroupextid"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/usergroups/extid/{usergroupextid}/identities")]
        [HttpGet]
        [ProducesResponseType(typeof(IdentityStatusDto),200)]
        public IActionResult GetListUserIdentityStatusByExtId(int ownerId, string usergroupextid)
        {
            var userGroupDtos = _userGroupService.GetListUserGroupIdentityStatusByExtId(usergroupextid);
            if (userGroupDtos == null)
            {
                return CreateNotFoundResponse(string.Format("No UserGroup found: UserExtId({0})", usergroupextid));
            }
            return CreateResponse(userGroupDtos);
        }

        /// <summary>
        /// Get user group by id
        /// </summary>
        /// <param name="usergroupId"></param>
        /// <returns></returns>
        [Route("usergroups/{usergroupId}/identity")]
        [HttpGet]
        [ProducesResponseType(typeof(IdentityStatusDto),200)]
        public IActionResult GetUserGroupIdentityStatusById(int usergroupId)
        {
            var userGroupDto = _userGroupService.GetUserGroups<IdentityStatusDto>(userGroupIds: new List<int> { usergroupId },
                  statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            if (userGroupDto == null)
            {
                return CreateNotFoundResponse(string.Format("UserGroup not found: usergroupId({0})", usergroupId));
            }
            return CreateResponse(userGroupDto);
        }
    }
}
