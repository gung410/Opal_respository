using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Business.Queries.ApprovingOfficer;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Validators;
using cxOrganization.WebServiceAPI.ActionFilters;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace cxOrganization.WebServiceAPI.Controllers
{
    /// <summary>
    /// approval group API 
    /// </summary>
    [Authorize]
    public class ApprovalGroupsController : ApiControllerBase
    {
        private readonly IApprovalGroupMemberService _approvalGroupMemberService;
        private readonly IUserGroupService _userGroupService;
        private readonly IUserService _userService;
        private readonly SearchApprovingOfficersQueryHandler _searchApprovingOfficersQueryHandler;
        private readonly ILogger _logger;
        private readonly IAdvancedWorkContext _workContex;

        /// <summary>
        /// Constructor
        /// </summary>
        public ApprovalGroupsController( ILogger<ApprovalGroupsController> logger, IApprovalGroupMemberService approvalGroupMemberService,
            IAdvancedWorkContext workContext,
            Func<ArchetypeEnum, IUserGroupService> userGroupService,
            Func<ArchetypeEnum, IUserService> userService,
            SearchApprovingOfficersQueryHandler searchApprovingOfficersQueryHandler)
        {
            _logger = logger;
            _approvalGroupMemberService = approvalGroupMemberService;
            _userGroupService = userGroupService(ArchetypeEnum.ApprovalGroup);
            _userService = userService(ArchetypeEnum.Employee);
            _searchApprovingOfficersQueryHandler = searchApprovingOfficersQueryHandler;
            _workContex = workContext;
        }

        /// <summary>
        /// get all ApprovalGroups
        /// </summary>
        /// <param name="approvalGroupIds"></param>
        /// <param name="statusEnums"></param>
        /// <param name="extIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></para
        /// <param name="parentDepartmentId">Parent department</param>
        /// <param name="groupTypes"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <param name="searchInSameDepartment"></param>
        /// <param name="assigneeDepartmentId"></param>
        /// <param name="searchInSameDepartment"></param>
        /// <param name="searchFromDepartmentToTop"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="searchKey"></param>
        [Route("approvalgroups")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ApprovalGroupDto>), 200)]
        public async Task<IActionResult> GetApprovalGroupsByParams(
            int parentDepartmentId = 0,
            [FromQuery] List<int> approvalGroupIds = null,
            [FromQuery] List<int> approverIds = null,
            [FromQuery] List<int> employeeIds = null,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            [FromQuery] List<EntityStatusEnum> userStatusEnums = null,
            [FromQuery] List<GrouptypeEnum> groupTypes = null,
            [FromQuery] List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            int? assigneeDepartmentId = null,
            bool searchInSameDepartment = false,
            bool searchFromDepartmentToTop = false,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            string searchKey = null)
        {

            var pagingDto = await _searchApprovingOfficersQueryHandler.HandleAsync(new SearchApprovingOfficersQuery
            {
                ApprovalGroupIds = approvalGroupIds,
                ApproverIds = approverIds,
                AssigneeDepartmentId = assigneeDepartmentId,
                EmployeeIds = employeeIds,
                ExtIds = extIds,
                GroupTypes = groupTypes,
                LastUpdatedAfter = lastUpdatedAfter,
                LastUpdatedBefore = lastUpdatedBefore,
                OrderBy = orderBy,
                PageIndex = pageIndex,
                PageSize = pageSize,
                ParentDepartmentId = parentDepartmentId,
                SearchFromDepartmentToTop = searchFromDepartmentToTop,
                SearchInSameDepartment = searchInSameDepartment,
                SearchKey = searchKey,
                StatusEnums = statusEnums,
                UserStatusEnums = userStatusEnums
            });

            return CreatePagingResponse(pagingDto.Items,
                pagingDto.PageIndex,
                pagingDto.PageSize,
                pagingDto.HasMoreData);
        }

        /// <summary>
        /// Insert approval group
        /// </summary>
        /// <param name="approvalGroup"></param>
        /// <returns></returns>
        [Route("approvalgroups")]
        [HttpPost]
        [ProducesResponseType(typeof(ApprovalGroupDto), 200)]
        [TypeFilter(typeof(PreventXSSFilter))]
        public IActionResult AddNewApprovalGroup([FromBody]ApprovalGroupDto approvalGroup)
        {
            // Since the get users method has implemented the check access on the being retrieved users
            // If the service doesn't return any user, then the logged-in user doesn't have access on the approver.
            var approver = _userService.GetUsers<EmployeeDto>(
                ownerId: approvalGroup.Identity.OwnerId,
                customerIds: new List<int> {approvalGroup.Identity.CustomerId},
                userIds: new List<int> {approvalGroup.ApproverId.Value},
                statusIds: new List<EntityStatusEnum> {EntityStatusEnum.All}).Items;
            if (approver.Count == 0)
            {

                _logger.LogWarning($"Logged-in user with sub '{_workContex.Sub}' does not have access on user {approvalGroup.ApproverId} to create approvalgroups.");
                return AccessDenied();
            }

            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            approvalGroup.Identity.Id = 0;
            var group = _userGroupService.InsertUserGroup(validationSpecification, approvalGroup);
            return StatusCode((int)HttpStatusCode.Created, group);
        }

        /// <summary>
        /// get ApprovalGroup
        /// </summary>
        /// <param name="approvalgroupId"></param>
        /// <returns></returns>
        [Route("approvalgroups/{approvalgroupid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ApprovalGroupDto), 200)]
        public async Task<IActionResult> GetApprovalGroupById(int approvalgroupid)
        {

            var approvalGroup = (await _searchApprovingOfficersQueryHandler.HandleAsync(new SearchApprovingOfficersQuery
            {
                ApprovalGroupIds = new List<int> { approvalgroupid },
                StatusEnums = new List<EntityStatusEnum> {EntityStatusEnum.All}

            })).Items.FirstOrDefault();

            if (approvalGroup == null)
                return CreateNotFoundResponse<ApprovalGroupDto>();
            return CreateResponse<ApprovalGroupDto>(approvalGroup);

        }

        /// <summary>
        /// Update approval group
        /// </summary>
        /// <param name="approvalgroupid"></param>
        /// <param name="approvalGroup"></param>
        /// <returns></returns>
        [Route("approvalgroups/{approvalgroupid}")]
        [HttpPut]
        [ProducesResponseType(typeof(ApprovalGroupDto), 200)]
        [TypeFilter(typeof(PreventXSSFilter))]
        public async Task<IActionResult> UpdateApprovalGroupInfo(int approvalgroupid, [FromBody]ApprovalGroupDto approvalGroup)
        {
            // Since the get users method has implemented the check access on the being retrieved users
            // If the service doesn't return any user, then the logged-in user doesn't have access on the approver.
            var approver = (await _userService.GetUsersAsync<EmployeeDto>(
                ownerId: approvalGroup.Identity.OwnerId,
                customerIds: new List<int> { approvalGroup.Identity.CustomerId },
                userIds: new List<int> { approvalGroup.ApproverId.Value },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All })).Items;

            if (approver.Count == 0)
            {
                _logger.LogWarning($"Logged-in user with sub '{_workContex.Sub}' does not have access on user {approvalGroup.ApproverId} to update approvalgroups.");
                return AccessDenied();
            }
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
           .WithStatus(EntityStatusEnum.All)
           .IsDirectParent()
           .Create();
            approvalGroup.Identity.Id = approvalgroupid;
            var group = (ApprovalGroupDto)_userGroupService.UpdateUserGroup(validationSpecification, approvalGroup);
            return CreateResponse<ApprovalGroupDto>(group);
        }

        //A member is a person belonging to a group, association, etc.
        //A membership is the verification(proof) that one is part of this group.

        /// <summary>
        /// Get approval group's learners
        /// </summary>
        /// <param name="approvalgroupid"></param>
        /// <param name="queryOptions"></param>
        /// <param name="statusEnums">Learner status enums</param>
        /// <returns></returns>
        [Route("approvalgroups/{approvalgroupid}/members")]
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeeDto>), 200)]
        public IActionResult GetApprovalGroupEmployees(int approvalgroupid,
            [FromQuery] List<EntityStatusEnum> statusEnums = null)
        {
            var employees = _userService.GetUsers<EmployeeDto>(userGroupIds: new List<int>() { approvalgroupid },
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Employee },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items;

            return base.CreateResponse<EmployeeDto>(employees);
        }
        /// <summary>
        /// Get approval group membership
        /// </summary>
        /// <param name="approvalgroupid"></param>
        /// <returns>list of approval group levels</returns>
        [Route("approvalgroups/{approvalgroupid}/memberships")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MemberDto>), 200)]
        public IActionResult GetApprovalGroupMembership(int approvalgroupid)
        {
            List<MemberDto> memberships = _approvalGroupMemberService.GetApprovalGroupMemberships(approvalgroupid);

            return CreateResponse<MemberDto>(memberships);
        }

    }
}
