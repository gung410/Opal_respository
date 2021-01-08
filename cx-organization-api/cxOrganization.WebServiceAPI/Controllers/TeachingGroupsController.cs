using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cxOrganization.WebServiceAPI.Controllers
{
    /// <summary>
    /// Teaching group API 
    /// </summary>
    [Authorize]
    public class TeachingGroupsController : ApiControllerBase
    {
        private readonly ITeachingGroupMemberService _teachingGroupMemberService;
        private readonly IWorkContext _workContext;
        private readonly IUserGroupService _userGroupService;
        private readonly OrganizationDbContext _organizationUnitOfWork;
        private readonly IUserService _userService;
        /// <summary>
        /// Constructor
        /// </summary>
        public TeachingGroupsController(ITeachingGroupMemberService teachingGroupMemberService,
            IWorkContext workContext,
            Func<ArchetypeEnum, IUserGroupService> userGroupService,
            Func<ArchetypeEnum, IUserService> userService,
            OrganizationDbContext unitOfWork)
        {
            _teachingGroupMemberService = teachingGroupMemberService;
            _workContext = workContext;
            _userGroupService = userGroupService(ArchetypeEnum.TeachingGroup);
            _userService = userService(ArchetypeEnum.Learner);
            _organizationUnitOfWork = unitOfWork;
        }

        /// <summary>
        /// get all TeachingGroups
        /// </summary>
        /// <param name="teachinggroupIds"></param>
        /// <param name="userIds"></param>
        /// <param name="statusEnums"></param>
        /// <param name="extIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="parentDepartmentId">Parent department</param>
        /// <param name="groupTypes"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selectIdentity"></param>
        /// <returns></returns>
        [Route("teachinggroups")]
        [HttpGet]
        [ProducesResponseType(typeof(List<TeachingGroupDto>), 200)]
        public IActionResult GetTeachingGroupsByParams(
            int parentDepartmentId = 0,
            [FromQuery] List<int> teachinggroupIds = null,
            [FromQuery] List<int> userIds = null,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            [FromQuery] List<GrouptypeEnum> groupTypes = null,
            [FromQuery] List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool selectIdentity = false,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var pagingDto = _userGroupService.GetUserGroups<TeachingGroupDto>(userGroupIds: teachinggroupIds,
                departmentIds: parentDepartmentId != 0 ? new List<int>() { parentDepartmentId } : null,
                memberUserIds: userIds, statusIds: statusEnums,
                archetypeIds: new List<int>() { (int)ArchetypeEnum.TeachingGroup },
                extIds: extIds,
                groupTypes: groupTypes,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items,
                pagingDto.PageIndex,
                pagingDto.PageSize,
                pagingDto.HasMoreData);
        }
        /// <summary>
        /// Insert teaching group
        /// </summary>
        /// <param name="teachingGroup"></param>
        /// <returns></returns>
        [Route("teachinggroups")]
        [HttpPost]
        [ProducesResponseType(typeof(TeachingGroupDto), 200)]
        public IActionResult AddNewTeachingGroup([FromBody]TeachingGroupDto teachingGroup)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment((int)teachingGroup.SchoolId, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            teachingGroup.Identity.Id = 0;
            var group = _userGroupService.InsertUserGroup(validationSpecification, teachingGroup);
            return StatusCode((int)HttpStatusCode.Created, group);
        }
        /// <summary>
        /// get TeachingGroup
        /// </summary>
        /// <param name="teachinggroupId"></param>
        /// <returns></returns>
        [Route("teachinggroups/{teachinggroupId}")]
        [HttpGet]
        [ProducesResponseType(typeof(TeachingGroupDto), 200)]
        public IActionResult GetTeachingGroupById(int teachinggroupId)
        {
            var teachingGroup = _userGroupService.GetUserGroups<TeachingGroupDto>(userGroupIds: new List<int>() { teachinggroupId },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.TeachingGroup },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            if (teachingGroup == null)
                return CreateNotFoundResponse<TeachingGroupDto>();
            return CreateResponse<TeachingGroupDto>(teachingGroup);

        }
        /// <summary>
        /// Update teaching group
        /// </summary>
        /// <param name="teachinggroupId"></param>
        /// <param name="teachingGroup"></param>
        /// <returns></returns>
        [Route("teachinggroups/{teachinggroupId}")]
        [HttpPut]
        [ProducesResponseType(typeof(TeachingGroupDto), 200)]
        public IActionResult UpdateTeachingGroupInfo(int teachinggroupId, [FromBody]TeachingGroupDto teachingGroup)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
           .ValidateDepartment((int)teachingGroup.SchoolId, ArchetypeEnum.School)
           .WithStatus(EntityStatusEnum.All)
           .IsDirectParent()
           .Create();
            teachingGroup.Identity.Id = teachinggroupId;
            var group = (TeachingGroupDto)_userGroupService.UpdateUserGroup(validationSpecification, teachingGroup);
            return CreateResponse<TeachingGroupDto>(group);
        }
        /// <summary>
        /// Get Teaching group's learners
        /// </summary>
        /// <param name="teachinggroupId"></param>
        /// <param name="queryOptions"></param>
        /// <param name="statusEnums">Learner status enums</param>
        /// <returns></returns>
        [Route("teachinggroups/{teachinggroupId}/learners")]
        [HttpGet]
        [ProducesResponseType(typeof(List<LearnerDto>), 200)]
        public IActionResult GetTeachingGroupLearners(int teachinggroupId,
             [FromQuery] List<EntityStatusEnum> statusEnums = null)
        {
            var learners = _userService.GetUsers<LearnerDto>(userGroupIds: new List<int>() { teachinggroupId },
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Learner },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items;

            return base.CreateResponse<LearnerDto>(learners);
        }

        /// <summary>
        /// Get Teaching group's learners
        /// </summary>
        /// <param name="teachinggroupId"></param>
        /// <param name="queryOptions"></param>
        /// <param name="statusEnums">Learner status enums</param>
        /// <returns></returns>
        [Route("teachinggroups/{teachinggroupId}/employees")]
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeeDto>), 200)]
        public IActionResult GetTeachingGroupEmployees(int teachinggroupId,
            [FromQuery] List<EntityStatusEnum> statusEnums = null)
        {
            var employees = _userService.GetUsers<EmployeeDto>(userGroupIds: new List<int>() { teachinggroupId },
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Employee },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items;

            return base.CreateResponse<EmployeeDto>(employees);
        }
        /// <summary>
        /// Get Teaching Group's learner by identifier
        /// </summary>
        /// <param name="teachinggroupId"></param>
        /// <param name="learnerid"></param>
        /// <returns></returns>
        [Route("teachinggroups/{teachinggroupId}/learners/{learnerId}")]
        [HttpGet]
        [ProducesResponseType(typeof(LearnerDto), 200)]
        public IActionResult GetTeachingGroupLearner(int teachinggroupId,
            int learnerId)
        {
            var learner = _userService.GetUsers<LearnerDto>(userIds: new List<int>() { learnerId }, userGroupIds: new List<int>() { teachinggroupId },
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Learner },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            return CreateResponse<LearnerDto>(learner);
        }
        /// <summary>
        /// Get teaching group membership
        /// </summary>
        /// <param name="teachinggroupId"></param>
        /// <returns>list of teaching group levels</returns>
        [Route("teachinggroups/{teachinggroupId}/memberships")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MemberDto>), 200)]
        public IActionResult GetTeachingGroupMembership(int teachinggroupId)
        {
            List<MemberDto> memberships = _teachingGroupMemberService.GetTeachingGroupMemberships(teachinggroupId);

            return CreateResponse<MemberDto>(memberships);
        }
        /// <summary>
        /// Add level to teaching group
        /// </summary>
        /// <param name="teachinggroupId"></param>
        /// <param name="levelDto"></param>
        /// <returns></returns>
        [Route("teachinggroups/{teachinggroupId}/memberships/levels")]
        [HttpPost]
        [ProducesResponseType(typeof(MemberDto), 200)]
        public IActionResult UpdateTeachingGroupsLevel(int teachinggroupId, [FromBody]MemberDto levelDto)
        {
            return CreateResponse(_teachingGroupMemberService.UpdateTeachingGroupLevel(teachinggroupId, levelDto));
        }
        /// <summary>
        /// remove teaching group's level
        /// </summary>
        /// <param name="teachinggroupId"></param>
        /// <param name="levelDto"></param>
        /// <returns></returns>
        [Route("teachinggroups/{teachinggroupId}/memberships/levels")]
        [HttpPut]
        [ProducesResponseType(typeof(MemberDto), 200)]
        public IActionResult RemoveTeachingGroupsLevel(int teachinggroupId, [FromBody]MemberDto levelDto)
        {
            return CreateResponse(_teachingGroupMemberService.RemoveTeachingGroupLevel(teachinggroupId, levelDto));
        }
        #region will be removed
        /// <summary>
        /// Create group (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <param name="schoolId"></param>
        /// <param name="teachingGroup"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}/schools/{schoolId}/teachinggroups")]
        [HttpPost]
        [ProducesResponseType(typeof(TeachingGroupDto), 200)]
        public IActionResult InsertTeachingGroup(int schoolownerId, int schoolId, TeachingGroupDto teachingGroup)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerId, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolId, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();

            teachingGroup.SchoolId = schoolId;
            teachingGroup.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            teachingGroup.Identity.Id = 0;
            var group = _userGroupService.InsertUserGroup(validationSpecification, teachingGroup);
            return CreateResponse(group);
        }

        /// <summary>
        /// Create group (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <param name="schoolId"></param>
        /// <param name="teachingGroup"></param>
        ///<param name="teachinggroupId"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}/schools/{schoolId}/teachinggroups/{teachinggroupId}")]
        [HttpPut]
        [ProducesResponseType(typeof(TeachingGroupDto), 200)]
        public IActionResult UpdateTeachingGroup(int schoolownerId, int schoolId, int teachinggroupId, TeachingGroupDto teachingGroup)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerId, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolId, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            teachingGroup.SchoolId = schoolId;
            teachingGroup.Identity.Id = teachinggroupId;
            teachingGroup.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            var group = _userGroupService.UpdateUserGroup(validationSpecification, teachingGroup);
            if (group == null)
                return CreateNotFoundResponse(string.Format("TeachingGroup not found: teachinggroupId({0}))", teachingGroup.Identity.Id));
            return CreateResponse(group);
        }

        /// <summary>
        /// Get group (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <param name="schoolId"></param>
        ///<param name="teachinggroupId"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}/schools/{schoolId}/teachinggroups/{teachinggroupId}")]
        [HttpGet]
        [ProducesResponseType(typeof(TeachingGroupDto), 200)]
        public IActionResult GetTeachingGroup(int schoolownerId, int schoolId, int teachinggroupId)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerId, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolId, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            var teachingGroupDto = _userGroupService.GetUserGroup(validationSpecification, teachinggroupId);
            if (teachingGroupDto != null)
                return CreateResponse(teachingGroupDto);
            else
                return CreateNotFoundResponse(string.Format("Teaching group not found: teachinggroupId({0})", teachinggroupId));
        }

        /// <summary>
        /// Get groups (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}/schools/{schoolId}/teachinggroups")]
        [HttpGet]
        [ProducesResponseType(typeof(List<TeachingGroupDto>), 200)]
        public IActionResult GetTeachingGroups(int schoolownerId, int schoolId)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerId, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolId, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            var teachingGroups = _userGroupService.GetUserGroups(validationSpecification, schoolId);

            if (teachingGroups.Any())
            {
                return CreateResponse(teachingGroups);
            }
            else
            {
                return CreateNotFoundResponse(string.Format("No teaching group found: schoolId({0})", schoolId));
            }
        }

        /// <summary>
        /// Get groups (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <param name="schoolId"></param>
        /// <param name="teachinggroupId"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}/schools/{schoolId}/teachinggroups/{teachinggroupId}")]
        [HttpDelete]
        public IActionResult DeleteTeachingGroup(int schoolownerId, int schoolId, int teachinggroupId)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerId, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolId, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            _userGroupService.DeleteUserGroupById(validationSpecification, teachinggroupId);
            _organizationUnitOfWork.SaveChanges();
            return CreateSuccessResponse();
        }

        /// <summary>
        /// Get group member (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <param name="schoolId"></param>
        /// <param name="teachinggroupId"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}/schools/{schoolId}/teachinggroups/{teachinggroupId}/members")]
        [HttpPost]
        [ProducesResponseType(typeof(MemberDto), 200)]
        public IActionResult AddMembers(int schoolownerId, int schoolId, int teachinggroupId, MemberDto memberDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerId, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolId, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            var member = _teachingGroupMemberService.AddMember(validationSpecification, teachinggroupId, memberDto);
            return CreateResponse(member);
        }

        /// <summary>
        /// Get group member (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <param name="schoolId"></param>
        /// <param name="teachinggroupId"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}/schools/{schoolId}/teachinggroups/{teachinggroupId}/members")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MemberDto>), 200)]
        public IActionResult GetMembers(int schoolownerId, int schoolId, int teachinggroupId)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerId, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolId, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            var members = _teachingGroupMemberService.GetMembers(validationSpecification, teachinggroupId);

            if (!members.Any())
                CreateNotFoundResponse(string.Format("Members not found: teachinggroupId({0})", teachinggroupId));
            return CreateResponse(members);
        }

        /// <summary>
        /// Get group member (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <param name="schoolId"></param>
        /// <param name="teachinggroupId"></param>
        /// <param name="memberid"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}/schools/{schoolId}/teachinggroups/{teachinggroupId}/members/{memberid}")]
        [HttpGet]
        [ProducesResponseType(typeof(MemberDto), 200)]
        public IActionResult GetMember(int schoolownerId, int schoolId, int teachinggroupId, int memberid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerId, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolId, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            var member = _teachingGroupMemberService.GetMember(validationSpecification, teachinggroupId, memberid);

            if (member == null)
                CreateNotFoundResponse(string.Format("Member not found: memberId({0})", memberid));
            return CreateResponse(member);
        }

        /// <summary>
        /// Get group membership (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <param name="schoolId"></param>
        /// <param name="teachinggroupId"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}/schools/{schoolId}/teachinggroups/{teachinggroupId}/memberships")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MemberDto>), 200)]
        public IActionResult GetMemberships(int schoolownerId, int schoolId, int teachinggroupId)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerId, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolId, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            var memberships = _teachingGroupMemberService.GetUserGroupMemberShip(validationSpecification, teachinggroupId);

            if (memberships.Any())
                CreateNotFoundResponse(string.Format("Membership(s) not found: teachinggroupId({0})", teachinggroupId));
            return CreateResponse(memberships);
        }
        #endregion
    }
}
