using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using cxOrganization.Business.DeactivateOrganization.DeactivateUser;
using cxOrganization.Client;
using cxOrganization.Client.Departments;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Settings;
using cxOrganization.Domain.Validators;
using cxOrganization.WebServiceAPI.Auth;
using cxOrganization.WebServiceAPI.Extensions;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace cxOrganization.WebServiceAPI.Controllers
{
    /// <summary>
    /// Candidate API 
    /// </summary>
    [Authorize]
    public class CandidatesController : ApiControllerBase
    {
        //private readonly IUserGroupUserMemberService _userGroupUserMemberService;
        private readonly IUserService _userService;
        private readonly IUserGroupService _userGroupService;
        private readonly IWorkContext _workContext;
        private readonly IDepartmentService _departmentService;
        private readonly IUserTypeService _userTypeService;
        private readonly bool _checkSingleUserAccess = false;
        private readonly Func<ArchetypeEnum, IUGMemberService> _userGroupUserMemberService;
        private readonly ILoginServiceUserService _loginServiceUserService;
        private readonly IDeactivateUserService<CandidateDto> _deactivateUserService;
        private readonly AppSettings _appSettings;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userGroupService"></param>
        /// <param name="departmentService"></param>
        /// <param name="workContext"></param>
        /// <param name="candidatePoolMemberService">This class responsible for add membership between candidate and candidate pool </param>
        /// <param name="userGroupUserMemberService"></param>
        /// <param name="userTypeService"></param>
        /// <param name="loginServiceUserService"></param>
        public CandidatesController(Func<ArchetypeEnum, IUserService> userService,
            Func<ArchetypeEnum, IUserGroupService> userGroupService,
            Func<ArchetypeEnum, IDepartmentService> departmentService,
            IWorkContext workContext,
            Func<ArchetypeEnum, IUGMemberService> userGroupUserMemberService,
            ICandidatePoolMemberService candidatePoolMemberService,
            Func<ArchetypeEnum, IUserTypeService> userTypeService,
            ILoginServiceUserService loginServiceUserService,
            IDeactivateUserService<CandidateDto> deactivateUserService,
            IOptions<AppSettings> options)
        {
            _userService = userService(ArchetypeEnum.Candidate);
            _userGroupService = userGroupService(ArchetypeEnum.CandidatePool);
            _departmentService = departmentService(ArchetypeEnum.Country);
            _workContext = workContext;
            _userGroupUserMemberService = userGroupUserMemberService;
            _userTypeService = userTypeService(ArchetypeEnum.Candidate);
            _loginServiceUserService = loginServiceUserService;
            _deactivateUserService = deactivateUserService;
            _checkSingleUserAccess = options.Value.CheckSingleUserAccess;
            _appSettings = options.Value;
        }

        /// <summary>
        /// Get Candidates
        /// </summary>
        /// <param name="parentDepartmentIds"></param>
        /// <param name="candidateIds"></param>
        /// <param name="userGroupIds"></param>
        /// <param name="statusEnums">Candidate StatusEnum: Default is Active</param>
        /// <param name="extIds"></param>
        /// <param name="ssnList"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="getDynamicProperties"></param>
        /// <param name="orderBy"></param>
        /// <param name="loginServiceIds"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selectIdentity"></param>
        /// <param name="filterOnParentHd"></param>
        /// <param name="extId"></param>
        /// <param name="ageRanges"></param>
        /// <param name="genders"></param>
        /// <param name="searchKey"></param>
        /// <param name="ssn"></param>
        /// <param name="userNames"></param>
        /// <param name="memberValidFromBefore">Filter on UGmember Valid From(Just work when filter by userGroupIds)</param>
        /// <param name="memberValidFromAfter">Filter on UGmember Valid From(Just work when filter by userGroupIds)</param>
        /// <param name="memberValidToBefore">Filter on UGmember Valid To(Just work when filter by userGroupIds)</param>
        /// <param name="memberValidToAfter">Filter on UGmember Valid To(Just work when filter by userGroupIds)</param>
        /// <param name="memberStatuses">Filter on UGMember Status(Just work when filter by userGroupIds)</param>
        /// <param name="getLoginServiceClaims"></param>
        /// <param name="getRoles"></param>
        /// <param name="loginServiceClaims"></param>
        /// <param name="loginServiceClaimTypes"></param>
        /// <param name="roleIds">List of id of role. (Role id is id of UserType now)</param>
        /// <param name="roleExtIds">List of extId of role. (Role ExtId is extid of UserType now)</param>
        /// <returns></returns>
        [Route("candidates", Name = "candidates:get_by_parameter")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidateDto>),200)]
        public IActionResult GetCandidates(
            [FromQuery] List<int> parentDepartmentIds = null,
            [FromQuery] List<int> candidateIds = null,
            [FromQuery] List<int> userGroupIds = null,        
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            [FromQuery] List<string> extIds = null,
            [FromQuery] List<AgeRange> ageRanges = null,
            [FromQuery] List<Gender> genders = null,
            [FromQuery] List<EntityStatusEnum> memberStatuses = null,
            [FromQuery] DateTime? memberValidFromBefore = null,
            [FromQuery] DateTime? memberValidFromAfter = null,
            [FromQuery] DateTime? memberValidToBefore = null,
            [FromQuery] DateTime? memberValidToAfter = null,
            string searchKey = null,
            string extId = null,
            [FromQuery]List<string> ssnList = null,
            string ssn = null,
            [FromQuery] List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? getDynamicProperties = null,
            bool? getLoginServiceClaims = null,
            bool? getRoles = null,
            [FromQuery] List<string> loginServiceClaims = null,
            [FromQuery] List<string> loginServiceClaimTypes = null,
            [FromQuery] List<int> loginServiceIds = null,
            [FromQuery] List<int> roleIds = null,
            [FromQuery] List<string> roleExtIds = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            bool selectIdentity = false,
            bool? filterOnParentHd = true,
            bool? filterOnSubDepartment = null)
        {
            if (!string.IsNullOrEmpty(extId))
            {
                extIds = extIds ?? new List<string>();
                extIds.Add(extId);
            }
            if (!string.IsNullOrEmpty(ssn))
            {
                ssnList = ssnList ?? new List<string>();
                ssnList.Add(ssn);
            }
            if (Request.ShouldLimitPageSize())
            {
                pageSize = _appSettings.LimitUserPageSize(pageSize);
            }
            var pagingDto = _userService.GetUsers<CandidateDto>(parentDepartmentIds: parentDepartmentIds,
                   userIds: candidateIds,
                   userGroupIds: userGroupIds,
                   statusIds: statusEnums,
                   archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Candidate },
                   userTypeIds: roleIds,
                   userTypeExtIds: roleExtIds,
                   extIds: extIds, ssnList: ssnList,
                   lastUpdatedBefore: lastUpdatedBefore,
                   lastUpdatedAfter: lastUpdatedAfter,
                   getDynamicProperties: getDynamicProperties,
                   getLoginServiceClaims: getLoginServiceClaims,
                   searchKey: searchKey,
                   memberStatuses: memberStatuses,
                   memberValidFromAfter: memberValidFromAfter,
                   memberValidFromBefore: memberValidFromBefore,
                   memberValidToAfter: memberValidToAfter,
                   memberValidToBefore: memberValidToBefore,
                   ageRanges: ageRanges,
                   genders: genders,
                   pageSize: pageSize,
                   pageIndex: pageIndex,
                   userNames: userNames,
                   loginServiceClaims: loginServiceClaims,
                   loginServiceClaimTypes: loginServiceClaimTypes,
                   getRoles: getRoles,
                   loginServiceIds: loginServiceIds,
                   orderBy: orderBy,
                   filterOnParentHd: filterOnParentHd,
                   filterOnSubDepartment: filterOnSubDepartment);

            var items = pagingDto.Items;
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers[AuthenticationDefaults.AuthorizationHeader]);
            if (_checkSingleUserAccess && authHeader.Scheme == "Bearer")
            {
                items = pagingDto.Items.Where(x => x.Identity.Id == _workContext.CurrentUserId).ToList();
            }
            return CreatePagingResponse(items,
                
                pagingDto.PageIndex,
                pagingDto.PageSize,
                pagingDto.HasMoreData,
                selectIdentity);
        }

        /// <summary>
        /// Insert candidate
        /// </summary>
        /// <param name="candidateDto"></param>
        /// <returns></returns>
        [Route("candidates", Name = "candidates:insert")]
        [HttpPost]
        [ProducesResponseType(typeof(CandidateDto),200)]
        public IActionResult InsertCandidates([FromBody]CandidateDto candidateDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
               .ValidateDepartment(candidateDto.ParentDepartmentId, ArchetypeEnum.Unknown)
               .SkipCheckingArchetype()
               .WithStatus(EntityStatusEnum.All)
               .IsDirectParent()
               .Create();
            candidateDto.Identity.Id = 0;
            var userDtoBase = _userService.InsertUser(validationSpecification, candidateDto);
            return StatusCode((int)HttpStatusCode.Created, userDtoBase);
        }

        /// <summary>
        /// Get candidate by identifier
        /// </summary>
        /// <param name="candidateid"></param>
        /// <returns></returns>
        [Route("candidates/{candidateid}", Name = "candidates:get")]
        [HttpGet]
        [ProducesResponseType(typeof(CandidateDto),200)]
        public IActionResult GetCandidate(int candidateid)
        {
            var candidate = _userService.GetUsers<CandidateDto>(userIds: new List<int>() { candidateid },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            if(candidate == null)
            {
                return CreateNotFoundResponse<CandidateDto>();
            }
            return CreateResponse(candidate);
        }

        /// <summary>
        /// Update candidate
        /// </summary>
        /// <param name="candidateid"></param>
        /// <param name="candidateDto"></param>
        /// <param name="skipCheckingEntityVersion"></param>
        /// <returns></returns>
        [Route("candidates/{candidateid}", Name = "candidates:update")]
        [HttpPut]
        [ProducesResponseType(typeof(CandidateDto),200)]
        public IActionResult UpdateCandidate(int candidateid, [FromBody]CandidateDto candidateDto, bool skipCheckingEntityVersion = false)
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers[AuthenticationDefaults.AuthorizationHeader]);
            if (candidateid != _workContext.CurrentUserId && _checkSingleUserAccess && authHeader.Scheme == "Bearer")
                return StatusCode((int)System.Net.HttpStatusCode.Forbidden, "Access denied to candidate id: " + candidateid);
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(candidateDto.ParentDepartmentId, ArchetypeEnum.Unknown)
                .SkipCheckingArchetype()
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();

            candidateDto.Identity.Id = candidateid;
            var candidate = _userService.UpdateUser(validationSpecification, candidateDto, skipCheckingEntityVersion);
            return CreateResponse(candidate);
        }

        /// <summary>
        /// Get all Candidate pools that candidate belongs to
        /// </summary>
        /// <param name="candidateid"></param>
        /// <param name="statusEnums">candidate pool status enums</param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [Route("candidates/{candidateid}/candidatepools")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidatePoolDto>),200)]
        public IActionResult GetCandidatepool(int candidateid,
            [FromQuery] List<EntityStatusEnum> statusEnums = null)
        {
            var candidatepools = _userGroupService.GetUserGroups<CandidatePoolDto>(
                memberUserIds: new List<int>() { candidateid },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.CandidatePool },
                statusIds: statusEnums).Items;

            return base.CreateResponse<CandidatePoolDto>(candidatepools);
        }

        /// <summary>
        /// Get candidatepool by candidatepoolid which candaidate belong 
        /// </summary>
        /// <param name="candidateid"></param>
        /// <param name="candidatepoolid"></param>
        /// <returns></returns>
        [Route("candidates/{candidateid}/candidatepools/{candidatepoolid}")]
        [HttpGet]
        [ProducesResponseType(typeof(CandidatePoolDto),200)]
        public IActionResult GetCandidatepoolBelong(int candidateid,
            int candidatepoolid)
        {
            var candidatepool = _userGroupService.GetUserGroups<CandidatePoolDto>(userGroupIds: new List<int>() { candidatepoolid },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.CandidatePool },
                memberUserIds: new List<int>() { candidateid },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            if (candidatepool != null)
            {
                return CreateResponse(candidatepool);
            }
            else
            {
                return CreateNotFoundResponse(string.Format("No Candidatepool found: Candidateid({0})", candidateid));
            }
        }

        /// <summary>
        /// Get all countries that candidate belongs to
        /// </summary>
        /// <param name="candidateid"></param>
        /// <param name="statusEnums">country status enums</param>
        /// <returns></returns>
        [Route("candidates/{candidateid}/countries")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CountryDto>),200)]
        public IActionResult GetcountriesBelong(int candidateid,
            [FromQuery] List<EntityStatusEnum> statusEnums = null)
        {
            var countries = _departmentService.GetDepartments<CountryDto>(userIds: new List<int>() { candidateid },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.Country },
                statusIds: statusEnums).Items;

            return CreateResponse<CountryDto>(countries);

        }

        /// <summary>
        /// Get country which candidate belongs to
        /// </summary>
        /// <param name="candidateid"></param>
        /// <param name="countryid"></param>
        /// <returns></returns>
        [Route("candidates/{candidateid}/countries/{countryid}")]
        [HttpGet]
        [ProducesResponseType(typeof(CountryDto),200)]
        public IActionResult GetCountryBelong(int candidateid, int countryid)
        {
            var country = _departmentService.GetDepartments<CountryDto>(userIds: new List<int>() { candidateid },
                departmentIds: new List<int>() { countryid },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.Country },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            return CreateResponse<CountryDto>(country);
        }


        /// <summary>
        /// Add candidate to candidate pool
        /// </summary>
        /// <param name="candidateId"></param>
        /// <param name="candidatePoolMDto"></param>
        /// <returns></returns>
        [Route("candidates/{candidateid}/memberships/candidatepools")]
        [HttpPost]
        [ProducesResponseType(typeof(MemberDto),200)]
        public IActionResult InsertCandidatePoolMembership(int candidateId, [FromBody]MemberDto candidatePoolMDto)
        {
            return CreateResponse(_userGroupUserMemberService(ArchetypeEnum.CandidatePool).InsertUserGroupUserMembership(candidateId, candidatePoolMDto));
        }

        /// <summary>
        /// Remove candidate on candidate pool
        /// </summary>
        /// <param name="candidateId"></param>
        /// <param name="candidatePoolMDto"></param>
        /// <returns></returns>
        [Route("candidates/{candidateid}/memberships/candidatepools")]
        [HttpPut]
        [ProducesResponseType(typeof(MemberDto),200)]
        public IActionResult UpdateCandidatePoolMembership(int candidateId, MemberDto candidatePoolMDto)
        {
            return CreateResponse(_userGroupUserMemberService(ArchetypeEnum.CandidatePool).UpdateUserGroupUserMembership(candidateId, candidatePoolMDto));
        }
        #region will be removed 
        /// <summary>
        /// Get candidate on data owner
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/candidates")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidateDto>),200)]
        public IActionResult GetCandidatesOnDataOwner(int dataownerid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();

            return CreateResponse(_userService.GetUsers(validationSpecification, true));
        }
        /// <summary>
        /// Get candidate on data owner
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="candidateid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/candidates/{candidateid}", Name = "candidates:get_on_dataowner")]
        [HttpGet]
        [ProducesResponseType(typeof(CandidateDto),200)]
        public IActionResult GetCandidateOnDataowner(int dataownerid, int candidateid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();

            var candidate = _userService.GetUser(validationSpecification, candidateid);
            if (candidate != null)
            {
                return CreateResponse(candidate);
            }
            else
            {
                return CreateNotFoundResponse(string.Format("Candidate not found: CandidateId({0})", candidateid));
            }
        }
        /// <summary>
        /// Get candidate on country
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="countryid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/countries/{countryid}/candidates")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidateDto>),200)]
        public IActionResult GetCandidatesOnCountry(int dataownerid, int countryid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
                .ValidateDepartment(countryid, ArchetypeEnum.Country)
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();
            return CreateResponse(_userService.GetUsers(validationSpecification, true));
        }
        /// <summary>
        /// Insert candidate on data owner
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="candidateDto"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/candidates", Name = "candidates:insert_on_dataowner")]
        [HttpPost]
        [ProducesResponseType(typeof(CandidateDto),200)]
        public IActionResult InsertCandidatesOnDataOwner(int dataownerid, [FromBody]CandidateDto candidateDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();
            candidateDto.Identity.Id = 0;

            candidateDto.ParentDepartmentId = dataownerid;
            candidateDto.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            var userDtoBase = _userService.InsertUser(validationSpecification, candidateDto);
            return CreateResponse(userDtoBase);
        }

        /// <summary>
        /// Get candidate on country 
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="countryid"></param>
        /// <param name="candidateid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/countries/{countryid}/candidates/{candidateid}", Name = "candidates:get_on_country")]
        [HttpGet]
        [ProducesResponseType(typeof(CandidateDto),200)]
        public IActionResult GetCandidateOnCountry(int dataownerid, int countryid, int candidateid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
                .ValidateDepartment(countryid, ArchetypeEnum.Country)
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();

            var candidate = _userService.GetUser(validationSpecification, candidateid);
            if (candidate != null)
            {
                return CreateResponse(candidate);
            }
            else
            {
                return CreateNotFoundResponse(string.Format("Candidate not found: CandidateId({0})", candidateid));
            }
        }


        /// <summary>
        /// Update candidate on data owner
        /// </summary>
        /// <param name="candidateid"></param>
        /// <param name="dataownerid"></param>
        /// <param name="candidateDto"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/candidates/{candidateid}", Name = "candidates:update_on_dataowner")]
        [HttpPut]
        [ProducesResponseType(typeof(CandidateDto),200)]
        public IActionResult UpdateCandidateOnDataOwner(int candidateid, int dataownerid, [FromBody]CandidateDto candidateDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();

            candidateDto.Identity.Id = candidateid;
            candidateDto.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            var candidate = _userService.UpdateUser(validationSpecification, candidateDto);
            if (candidate == null)
                return CreateNotFoundResponse(string.Format("Candidate not found: CandidateId({0}))", candidateid));
            else
            {
                return CreateResponse(candidate);
            }
        }

        /// <summary>
        /// Update candidate on country
        /// </summary>
        /// <param name="candidateid"></param>
        /// <param name="dataownerid"></param>
        /// <param name="countryid"></param>
        /// <param name="candidateDto"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/countries/{countryid}/candidates/{candidateid}", Name = "candidates:update_on_country")]
        [HttpPut]
        [ProducesResponseType(typeof(CandidateDto),200)]
        public IActionResult UpdateCandidateOnCountry(int candidateid, int dataownerid, int countryid, [FromBody]CandidateDto candidateDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
                .ValidateDepartment(countryid, ArchetypeEnum.Country)
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();

            candidateDto.Identity.Id = candidateid;
            candidateDto.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            var candidate = _userService.UpdateUser(validationSpecification, candidateDto);
            if (candidate == null)
                return CreateNotFoundResponse(string.Format("Candidate not found: CandidateId({0}))", candidateid));
            else
            {
                return CreateResponse(candidate);
            }
        }
        /// <summary>
        /// Get candidate memberships on data owner
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="candidateid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/candidates/{candidateid}/memberships")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MemberDto>),200)]
        public IActionResult GetCandidateMembershipsOnDataOwner(int dataownerid, int candidateid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();

            var memberships = _userService.GetUserMemberships(validationSpecification, candidateid, ArchetypeEnum.Candidate, ArchetypeEnum.Unknown, ArchetypeEnum.Unknown);

            if (memberships.Any())
            {
                return CreateResponse(memberships);
            }
            else
            {
                return CreateNotFoundResponse(string.Format("No membership found: Candidateid({0})", candidateid));
            }
        }

        /// <summary>
        /// Get candidate memberships on country
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="countryid"></param>
        /// <param name="candidateid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/countries/{countryid}/candidates/{candidateid}/memberships")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MemberDto>),200)]
        public IActionResult GetCandidateMembershipsOnCountry(int dataownerid, int countryid, int candidateid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
                .ValidateDepartment(countryid, ArchetypeEnum.Country)
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();
            var memberships = _userService.GetUserMemberships(validationSpecification, candidateid, ArchetypeEnum.Candidate, ArchetypeEnum.Unknown, ArchetypeEnum.Unknown);

            if (memberships.Any())
            {
                return CreateResponse(memberships);
            }
            else
            {
                return CreateNotFoundResponse(string.Format("No membership found: Candidateid({0})", candidateid));
            }
        }
        #endregion
        /// <summary>
        /// Add usertypes for user
        /// </summary>
        /// <param name="candidateid"></param>
        /// <param name="memberDto">usertypes info</param>
        /// <returns></returns>
        [Route("candidates/{candidateid}/memberships/roles")]
        [HttpPost]
        [ProducesResponseType(typeof(MemberDto),200)]
        public IActionResult AddCandidateRole(int candidateid, [FromBody]MemberDto memberDto)
        {
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers[AuthenticationDefaults.AuthorizationHeader]);
                if (candidateid != _workContext.CurrentUserId && _checkSingleUserAccess && authHeader.Scheme == "Bearer")
                    return StatusCode((int)System.Net.HttpStatusCode.Forbidden, "Access denied to candidate id: " + candidateid);
                var responseMemberDto = _userTypeService.UpdateOrInsertUserTypeUser(candidateid,
                    memberDto, isUnique: false);
                return Ok(responseMemberDto);
            }
            catch (Exception ex)
            {
                if (ex is CXValidationException)
                    return BadRequest(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Remove candidate role
        /// </summary>
        /// <param name="candidateid"></param>
        /// <param name="memberDto">Holding role info</param>
        /// <returns></returns>
        [Route("candidates/{candidateid}/memberships/roles")]
        [HttpPut]
        [ProducesResponseType(typeof(MemberDto),200)]
        public IActionResult RemoveCandidateRole(int candidateid, [FromBody]MemberDto memberDto)
        {
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers[AuthenticationDefaults.AuthorizationHeader]);
                if (candidateid != _workContext.CurrentUserId && _checkSingleUserAccess && authHeader.Scheme == "Bearer")
                    return StatusCode((int)System.Net.HttpStatusCode.Forbidden, "Access denied to candidate id: " + candidateid);
                var responseMemberDto = _userTypeService.DeleteUserTypeUser(candidateid,
                memberDto);
                return Ok(responseMemberDto);
            }
            catch (Exception ex)
            {
                if (ex is CXValidationException)
                    return BadRequest(ex.Message);
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError,ex.Message);
            }
        }


        /// <summary>
        /// Get candidate's memberships
        /// </summary>
        /// <param name="candidateid"></param>
        /// <param name="archetypeIds">membership archetype enum</param>
        /// <param name="statusIds">membership status enum</param>
        /// <param name="membershipIds"></param>
        /// <param name="membershipExtIds"></param>
        /// <returns></returns>
        [Route("candidates/{candidateid}/memberships")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MemberDto>),200)]
        public IActionResult GetCandidateMemberships(int candidateid,
            [FromQuery] List<ArchetypeEnum> archetypeIds = null,
            [FromQuery] List<EntityStatusEnum> statusIds = null,
            [FromQuery] List<int> membershipIds = null,
            [FromQuery] List<string> membershipExtIds = null)
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers[AuthenticationDefaults.AuthorizationHeader]);
            if (candidateid != _workContext.CurrentUserId && _checkSingleUserAccess && authHeader?.Scheme == "Bearer")
                return StatusCode((int)System.Net.HttpStatusCode.Forbidden, "Access denied to candidate id: " + candidateid);
            var memberships = _userService.GetUserMemberships(candidateid,
                ArchetypeEnum.Candidate,
                membershipExtIds: membershipExtIds,
                membershipsArchetypeIds: archetypeIds,
                membershipIds: membershipIds,
                membershipStatusIds: statusIds);

            if (memberships.Any())
            {
                return CreateResponse(memberships);
            }
            else
            {
                return CreateNotFoundResponse(string.Format("No membership found: Candidateid ({0})", candidateid));
            }
        }
        /// <summary>
        /// Insert loginServiceUser that contains claim value for a candidate in a login service.
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        [Route("candidates/loginservices")]
        [HttpPost]
        [ProducesResponseType(typeof(LoginServiceUserDto),200)]
        //[ValidateIdentityCxToken]
        public IActionResult InsertLoginServiceClaim([Required] LoginServiceUserDto loginServiceUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (loginServiceUser.UserIdentity.Archetype != ArchetypeEnum.Candidate)
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, string.Format("UserIdentity with archetype '{0}' is not supported", loginServiceUser.UserIdentity.Archetype));
            var insertedLoginService = _loginServiceUserService.Insert(loginServiceUser);
            return StatusCode((int)HttpStatusCode.Created, insertedLoginService);
        }
        /// <summary>
        /// Update loginServiceUser that contains claim value for a candidate in a login service.
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        [Route("candidates/loginservices")]
        [HttpPut]
        [ProducesResponseType(typeof(LoginServiceUserDto),200)]
        //[ValidateIdentityCxToken]
        public IActionResult UpdateLoginServiceClaim([Required] LoginServiceUserDto loginServiceUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (loginServiceUser.UserIdentity.Archetype != ArchetypeEnum.Candidate)
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, string.Format("UserIdentity with archetype '{0}' is not supported", loginServiceUser.UserIdentity.Archetype));
            var updatedLoginService = _loginServiceUserService.Update(loginServiceUser);
            return StatusCode((int)HttpStatusCode.OK, updatedLoginService);
        }

        /// <summary>
        /// Delete loginServiceUser that contains claim value for a candidate in a login service.
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        [Route("candidates/loginservices")]
        [HttpDelete]
        [ProducesResponseType(typeof(LoginServiceUserDto),200)]
        //[ValidateIdentityCxToken]
        public IActionResult DeleteLoginServiceClaim([Required] LoginServiceUserDto loginServiceUser)
        {
       

            if (loginServiceUser.UserIdentity.Archetype != ArchetypeEnum.Candidate)
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, string.Format("UserIdentity with archetype '{0}' is not supported", loginServiceUser.UserIdentity.Archetype));
            var deletedLoginService = _loginServiceUserService.Delete(loginServiceUser);
            return StatusCode((int)HttpStatusCode.OK, deletedLoginService);
        }

        /// <summary>
        /// Get list of loginServiceUser of candidate based on given parameters
        /// </summary>
        /// <returns></returns>
        [Route("candidates/loginservices")]
        [HttpGet]
        [ProducesResponseType(typeof(List<LoginServiceUserDto>),200)]
        public IActionResult GetLoginServiceClaims(
            [FromQuery] List<int> candidateIds = null,
            [FromQuery] List<string> candidateExtIds = null,
            [FromQuery] List<int> loginServiceIds = null,
            [FromQuery] List<string> primaryClaimTypes = null,
            [FromQuery] List<EntityStatusEnum> candidateStatuses = null,
            [FromQuery] List<int> siteIds = null,
            [FromQuery] bool? includeLoginServiceHasNullSiteId = null,
            [FromQuery] List<string> claimValues = null,
            [FromQuery] DateTime? createdAfter = null,
            [FromQuery] DateTime? createdBefore = null)
        {

            var loginServiceUsers = _loginServiceUserService.Get(userIds: candidateIds,
                userExtIds: candidateExtIds,
                userArchetypes: new List<ArchetypeEnum> { ArchetypeEnum.Candidate },
                loginServiceIds: loginServiceIds,
                primaryClaimTypes: primaryClaimTypes,
                userEntityStatuses: candidateStatuses,
                siteIds: siteIds,
                includeLoginServiceHasNullSiteId: includeLoginServiceHasNullSiteId,
                claimValues: claimValues,
                createdAfter: createdAfter,
                createdBefore: createdBefore);

            return CreateResponse(loginServiceUsers);
        }

        /// <summary>
        /// Deactivate candidates
        /// </summary>
        /// <returns></returns>
        [Route("deactivate_candidates")]
        [HttpPut]
        [ProducesResponseType(typeof(List<DeactivateUsersResultDto>),200)]
       //[ValidateIdentityCxToken]
        public IActionResult DeactivateCandidates([FromBody] DeactivateCandidatesDto deactivateCandidates)
        {
            var deactivateCandidatesResult = _deactivateUserService.Deactivate(deactivateCandidates);
            var status = deactivateCandidatesResult.MaxStatus();
            if (status == 0)
                return StatusCode((int)HttpStatusCode.NoContent);
            var statusCode = status == (int) HttpStatusCode.NoContent
                ? HttpStatusCode.OK : (HttpStatusCode) status;
            return StatusCode((int)statusCode, deactivateCandidatesResult);
        }
    }
}