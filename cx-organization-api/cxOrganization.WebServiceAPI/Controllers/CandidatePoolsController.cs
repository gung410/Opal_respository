using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Client.Departments;
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
    /// CandidatePool API 
    /// </summary>
    [Authorize]
    public class CandidatePoolsController : ApiControllerBase
    {
        private readonly ICandidatePoolMemberService _candidatePoolMemberService;
        private readonly IWorkContext _workContext;
        private readonly IUserGroupService _userGroupService;
        private readonly IUserService _userService;
        private readonly OrganizationDbContext _organizationUnitOfWork;
        private readonly IDepartmentService _departmentService;
        private readonly Func<ArchetypeEnum, IDepartmentService> _departmentServiceDelegate;

        /// <summary>
        /// Constructor
        /// </summary>
        public CandidatePoolsController(ICandidatePoolMemberService candidatePoolMemberService,
            IWorkContext workContext, Func<ArchetypeEnum, IUserService> userService,
            Func<ArchetypeEnum, IUserGroupService> userGroupService,
            Func<ArchetypeEnum, IDepartmentService> departmentService,
            OrganizationDbContext unitOfWork)
        {
            _candidatePoolMemberService = candidatePoolMemberService;
            _workContext = workContext;
            _userService = userService(ArchetypeEnum.Candidate);
            _userGroupService = userGroupService(ArchetypeEnum.CandidatePool);
            _departmentService = departmentService(ArchetypeEnum.DataOwner);
            _departmentServiceDelegate = departmentService;
            _organizationUnitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get Candidate Pools
        /// </summary>
        /// <param name="candidatepoolIds"></param>
        /// <param name="userIds">List ids of user that is member of CandidatePool  </param>
        /// <param name="statusEnums">CandidatePools StatusEnums: Default is Active </param>
        /// <param name="extIds"></param>
        /// <param name="selectIdentity"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="extId"></param>
        /// <param name="departmentIds">List department ids that CandidatePool belong to</param>
        /// <param name="parentUserIds">List ids of user that CandidatePool belong to </param>
        /// <param name="groupTypes"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("candidatepools")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidatePoolDto>), 200)]
        public IActionResult GetCandidatepools(
            [FromQuery] List<int> candidatepoolIds = null,
            [FromQuery] List<int> departmentIds = null,
            [FromQuery] List<int> parentUserIds = null,
            [FromQuery] List<int> userIds = null,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            [FromQuery] List<GrouptypeEnum> groupTypes = null,
            [FromQuery] List<string> extIds = null,
            string extId = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool selectIdentity = false,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            if (!string.IsNullOrEmpty(extId))
            {
                extIds = extIds ?? new List<string>();
                extIds.Add(extId);
            }
            var pagingDto = _userGroupService.GetUserGroups<CandidatePoolDto>(
                departmentIds: departmentIds,
                groupUserIds: parentUserIds,
                userGroupIds: candidatepoolIds,
                memberUserIds: userIds,
                statusIds: statusEnums,
                archetypeIds: new List<int>() { (int)ArchetypeEnum.CandidatePool },
                extIds: extIds,
                groupTypes: groupTypes,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy
                );

            return CreatePagingResponse(pagingDto.Items,
                pagingDto.PageIndex,
                pagingDto.PageSize,
                pagingDto.HasMoreData);

        }
        /// <summary>
        /// Create CandidatePool
        /// </summary>
        /// <param name="candidatePoolDto"></param>
        /// <returns></returns>
        [Route("candidatepools")]
        [HttpPost]
        [ProducesResponseType(typeof(CandidatePoolDto), 200)]
        public IActionResult InsertCandidatePool([FromBody]CandidatePoolDto candidatePoolDto)
        {
            var validationSpecification = candidatePoolDto.ParentDepartmentId > 0
                ? (new HierarchyDepartmentValidationBuilder())
                        .ValidateDepartment((int)candidatePoolDto.ParentDepartmentId, ArchetypeEnum.Unknown)
                        .SkipCheckingArchetype()
                        .WithStatus(EntityStatusEnum.All)
                        .IsDirectParent()
                        .Create()
            : null;

            candidatePoolDto.Identity.Id = 0;
            var group = _userGroupService.InsertUserGroup(validationSpecification, candidatePoolDto);

            return CreateResponse(group);
        }

        /// <summary>
        /// Get candidate pool by identifier
        /// </summary>
        ///<param name="candidatepoolid"></param>
        /// <returns></returns>
        [Route("candidatepools/{candidatepoolid}")]
        [HttpGet]
        [ProducesResponseType(typeof(CandidatePoolDto), 200)]
        public IActionResult GetCandidatePool(int candidatepoolid)
        {
            var candidatePoolDto = _userGroupService.GetUserGroups<CandidatePoolDto>(userGroupIds: new List<int>() { candidatepoolid },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            if (candidatePoolDto == null)
                return CreateNotFoundResponse<CandidatePoolDto>();
            return CreateResponse(candidatePoolDto);
        }
        /// <summary>
        /// Update CandidatePool
        /// </summary>
        /// <param name="candidatePoolDto"></param>
        ///<param name="candidatepoolid"></param>
        /// <returns></returns>
        [Route("candidatepools/{candidatepoolid}")]
        [HttpPut]
        [ProducesResponseType(typeof(CandidatePoolDto), 200)]
        public IActionResult UpdateCandidatePool(int candidatepoolid, [FromBody]CandidatePoolDto candidatePoolDto)
        {
            var validationSpecification = candidatePoolDto.ParentDepartmentId > 0
                ? (new HierarchyDepartmentValidationBuilder())
                        .ValidateDepartment((int)candidatePoolDto.ParentDepartmentId, ArchetypeEnum.Unknown)
                        .SkipCheckingArchetype()
                        .WithStatus(EntityStatusEnum.All)
                        .IsDirectParent()
                        .Create()
            : null;

            candidatePoolDto.Identity.Id = candidatepoolid;
            var group = _userGroupService.UpdateUserGroup(validationSpecification, candidatePoolDto);

            return CreateResponse(group);
        }
        /// <summary>
        /// Get candidates that belong candidatepool
        /// </summary>
        /// <param name="candidatepoolid"></param>
        /// <param name="statusEnums"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="queryOptions"></param>
        /// <param name="selectIdentity"></param>
        /// <returns></returns>
        [Route("candidatepools/{candidatepoolid}/candidates")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidateDto>), 200)]
        public IActionResult GetCandidatesBelongCandidatePool(int candidatepoolid,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            bool selectIdentity = false,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var pagingDto = _userService.GetUsers<CandidateDto>(userGroupIds: new List<int>() { candidatepoolid },
                statusIds: statusEnums,
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Candidate },
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items,
                pagingDto.PageIndex,
                pagingDto.PageSize,
                pagingDto.HasMoreData);
        }
        /// <summary>
        /// Get Candidate that belongs CandidatePool
        /// </summary>
        /// <param name="candidatepoolid"></param>
        /// <param name="candidateid"></param>
        /// <returns></returns>
        [Route("candidatepools/{candidatepoolid}/candidates/{candidateid}")]
        [HttpGet]
        [ProducesResponseType(typeof(CandidateDto), 200)]
        public IActionResult GetCandidateBelongCandidatePool(int candidatepoolid,
            int candidateid)
        {
            var candidateDto = _userService.GetUsers<CandidateDto>(userIds: new List<int>() { candidateid }, archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Candidate },
                userGroupIds: new List<int>() { candidatepoolid },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            return base.CreateResponse<CandidateDto>(candidateDto);
        }

        /// <summary>
        /// get Data Owners that the candidate belong
        /// </summary>
        /// <param name="candidatepoolid"></param>
        /// <param name="statusEnums">DataOwner StatusEnums: Default is active</param>
        /// <returns></returns>
        [Route("candidatepools/{candidatepoolid}/dataowners")]
        [HttpGet]
        [ProducesResponseType(typeof(DataOwnerDto), 200)]
        public IActionResult GetDataOwner(int candidatepoolid,
            [FromQuery] List<EntityStatusEnum> statusEnums = null)
        {
            var dataowners = _departmentService.GetDepartments<DataOwnerDto>(userGroupIds: new List<int>() { candidatepoolid },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.DataOwner }, statusIds: statusEnums).Items.FirstOrDefault();

            return CreateResponse<DataOwnerDto>(dataowners);
        }
        /// <summary>
        /// Get Data Owner that the candidate belong
        /// </summary>
        /// <param name="candidatepoolid"></param>
        /// <param name="dataownerid"></param>
        /// <returns></returns>
        [Route("candidatepools/{candidatepoolid}/dataowners/{dataownerid}")]
        [HttpGet]
        [ProducesResponseType(typeof(DataOwnerDto), 200)]
        public IActionResult GetDataOwner(int candidatepoolid,
            int dataownerid)
        {
            var dataowner = _departmentService.GetDepartments<DataOwnerDto>(userGroupIds: new List<int>() { candidatepoolid },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.DataOwner },
                departmentIds: new List<int>() { dataownerid },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            return CreateResponse<DataOwnerDto>(dataowner);
        }
        /// <summary>
        /// Get companies that candidate belong to
        /// </summary>
        /// <param name="candidatepoolid"></param>
        /// <param name="statusEnums">company status enums</param>
        /// <param name="queryOptions"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("candidatepools/{candidatepoolid}/companies")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CompanyDto>), 200)]
        public IActionResult GetCompanies(int candidatepoolid,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var companieDtoPaging = _departmentServiceDelegate(ArchetypeEnum.Company)
                .GetDepartments<CompanyDto>(userGroupIds: new List<int>() { candidatepoolid },
                                archetypeIds: new List<int>() { (int)ArchetypeEnum.Company },
                                statusIds: statusEnums,
                                pageIndex: pageIndex,
                                pageSize: pageSize,
                                orderBy: orderBy);
            return base.CreatePagingResponse(companieDtoPaging.Items,
                companieDtoPaging.PageIndex,
                companieDtoPaging.PageSize,
                companieDtoPaging.HasMoreData);
        }
        /// <summary>
        /// get company that the candidate belong
        /// </summary>
        /// <param name="candidatepoolid"></param>
        /// <param name="companyid"></param>
        /// <returns></returns>
        [Route("candidatepools/{candidatepoolid}/companies/{companyid}")]
        [HttpGet]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        public IActionResult GetCompany(int candidatepoolid,
            int companyid)
        {
            var company = _departmentServiceDelegate(ArchetypeEnum.Company)
                .GetDepartments<CompanyDto>(userGroupIds: new List<int>() { candidatepoolid },
                                departmentIds: new List<int>() { companyid },
                                archetypeIds: new List<int>() { (int)ArchetypeEnum.Company },
                                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            return CreateResponse<CompanyDto>(company);
        }

        #region will be removed when version 6.9 is released
        /// <summary>
        /// Create CandidatePool
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="companyid"></param>
        /// <param name="candidatePoolDto"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/companies/{companyid}/candidatepools")]
        [HttpPost]
        [ProducesResponseType(typeof(CandidatePoolDto), 200)]
        public IActionResult InsertCandidatePool(int dataownerid, int companyid, CandidatePoolDto candidatePoolDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
            .ValidateDepartment(dataownerid, ArchetypeEnum.Company)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            candidatePoolDto.ParentDepartmentId = companyid;
            candidatePoolDto.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            candidatePoolDto.Identity.Id = 0;
            var group = _userGroupService.InsertUserGroup(validationSpecification, candidatePoolDto);
            return CreateResponse(group);
        }

        /// <summary>
        /// Update CandidatePool
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="companyid"></param>
        /// <param name="candidatePoolDto"></param>
        ///<param name="candidatepoolid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/companies/{companyid}/candidatepools/{candidatepoolid}")]
        [HttpPut]
        [ProducesResponseType(typeof(CandidatePoolDto), 200)]
        public IActionResult UpdateCandidatePool(int dataownerid, int companyid, int candidatepoolid, CandidatePoolDto candidatePoolDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
            .ValidateDepartment(dataownerid, ArchetypeEnum.Company)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            candidatePoolDto.ParentDepartmentId = companyid;
            candidatePoolDto.Identity.Id = candidatepoolid;
            candidatePoolDto.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            var group = _userGroupService.UpdateUserGroup(validationSpecification, candidatePoolDto);
            if (group == null)
                return CreateNotFoundResponse(string.Format("Candidate pool not found: candidatepoolid({0}))", candidatePoolDto.Identity.Id));
            return CreateResponse(group);
        }

        /// <summary>
        /// Get candidate pool
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="companyid"></param>
        ///<param name="candidatepoolid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/companies/{companyid}/candidatepools/{candidatepoolid}")]
        [HttpGet]
        [ProducesResponseType(typeof(CandidatePoolDto), 200)]
        public IActionResult GetCandidatePool(int dataownerid, int companyid, int candidatepoolid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
            .ValidateDepartment(dataownerid, ArchetypeEnum.Company)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            var candidatePoolDto = _userGroupService.GetUserGroup(validationSpecification, candidatepoolid);
            if (candidatePoolDto != null)
                return CreateResponse(candidatePoolDto);
            else
                return CreateNotFoundResponse(string.Format("Candidate pool not found: candidatepoolid({0})", candidatepoolid));
        }

        /// <summary>
        /// Get candidate pools
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="companyid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/companies/{companyid}/candidatepools")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidatePoolDto>), 200)]
        public IActionResult GetCandidatePools(int dataownerid, int companyid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
            .ValidateDepartment(companyid, ArchetypeEnum.Company)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            var candidatePools = _userGroupService.GetUserGroups(validationSpecification, companyid);

            if (candidatePools.Any())
            {
                return CreateResponse(candidatePools);
            }
            else
            {
                return CreateNotFoundResponse(string.Format("No candidate pool found: companyid({0})", companyid));
            }
        }

        /// <summary>
        /// Delete candidate pool
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="companyid"></param>
        /// <param name="candidatepoolid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/companies/{companyid}/candidatepools/{candidatepoolid}")]
        [HttpDelete]
        public IActionResult DeleteCandidatePool(int dataownerid, int companyid, int candidatepoolid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
            .ValidateDepartment(companyid, ArchetypeEnum.Company)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();

            _userGroupService.DeleteUserGroupById(validationSpecification, candidatepoolid);
            _organizationUnitOfWork.SaveChanges();
            return CreateSuccessResponse();
        }

        /// <summary>
        /// Get candidate pool member
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="companyid"></param>
        /// <param name="candidatepoolid"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/companies/{companyid}/candidatepools/{candidatepoolid}/members")]
        [HttpPost]
        [ProducesResponseType(typeof(MemberDto), 200)]
        public IActionResult AddMembers(int dataownerid, int companyid, int candidatepoolid, MemberDto memberDto)
        {
            var member = _candidatePoolMemberService.AddMember(candidatepoolid, memberDto);
            return CreateResponse(member);
        }

        /// <summary>
        /// Get candidate pool member
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="companyid"></param>
        /// <param name="candidatepoolid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/companies/{companyid}/candidatepools/{candidatepoolid}/members")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MemberDto>), 200)]
        public IActionResult GetMembers(int dataownerid, int companyid, int candidatepoolid)
        {

            var members = _candidatePoolMemberService.GetMembers(candidatepoolid);

            if (!members.Any())
                CreateNotFoundResponse(string.Format("No member found: candidatepoolid({0})", candidatepoolid));
            return CreateResponse(members);
        }

        /// <summary>
        /// Get group member
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="companyid"></param>
        /// <param name="candidatepoolid"></param>
        /// <param name="memberid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/companies/{companyid}/candidatepools/{candidatepoolid}/members/{memberid}")]
        [HttpGet]
        [ProducesResponseType(typeof(MemberDto), 200)]
        public IActionResult GetMember(int dataownerid, int companyid, int candidatepoolid, int memberid)
        {
            var member = _candidatePoolMemberService.GetMember(candidatepoolid, memberid);

            if (member == null)
                CreateNotFoundResponse(string.Format("Member not found: memberId({0})", memberid));
            return CreateResponse(member);
        }

        /// <summary>
        /// Get group membership
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="companyid"></param>
        /// <param name="candidatepoolid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/companies/{companyid}/candidatepools/{candidatepoolid}/memberships")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MemberDto>), 200)]
        public IActionResult GetMemberships(int dataownerid, int companyid, int candidatepoolid)
        {
            var memberships = _candidatePoolMemberService.GetUserGroupMemberShip(candidatepoolid);

            if (memberships.Any())
                CreateNotFoundResponse(string.Format("No membership found: candidatepoolid({0})", candidatepoolid));
            return CreateResponse(memberships);
        }
        #endregion
    }
}
