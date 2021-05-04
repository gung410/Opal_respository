using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using cxOrganization.Client.Departments;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.AdvancedWorkContext;
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
    /// Companies API 
    /// </summary>
    [Authorize]
    public class CompaniesController : ApiControllerBase
    {
        private readonly IDepartmentService _companydepartmentService;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IUserGroupService _userGroupService;
        private readonly IUserService _userService;

        /// <summary>
        /// Companies API Constructor
        /// </summary>
        /// <param name="departmentService"></param>
        /// <param name="workContext"></param>
        /// <param name="userGroupService"></param>
        /// <param name="userService"></param>
        public CompaniesController(Func<ArchetypeEnum, IDepartmentService> departmentService, 
            Func<ArchetypeEnum, IUserGroupService> userGroupService,
            Func<ArchetypeEnum, IUserService> userService,
            IAdvancedWorkContext workContext)
        {
            _companydepartmentService = departmentService(ArchetypeEnum.Company);
            _workContext = workContext;
            _userGroupService = userGroupService(ArchetypeEnum.CandidatePool);
            _userService = userService(ArchetypeEnum.Candidate);
        }

        /// <summary>
        /// Get companies
        /// </summary>
        /// <param name="companyIds"></param>
        /// <param name="parentDepartmentId"></param>
        /// <param name="childrenDepartmentId"></param>
        /// <param name="statusEnums">Company statusEnum : Default is active</param>
        /// <param name="extIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="extId">will be removed</param> 
        /// <param name="queryOptions"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selectIdentity"></param>
        /// <returns></returns>
        [Route("companies", Name = "companies:findall")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CompanyDto>),200)]
        public IActionResult GetCompanies([FromQuery] List<int> companyIds = null,
            int parentDepartmentId = 0,
            int childrenDepartmentId = 0,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            [FromQuery] List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            string extId = "",
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            bool selectIdentity=false)
        {
            if (!string.IsNullOrEmpty(extId))
            {
                extIds = extIds ?? new List<string>();
                extIds.Add(extId);
            }
            var pagingDto = _companydepartmentService.GetDepartments<CompanyDto>(departmentIds: companyIds,
                parentDepartmentId:parentDepartmentId,
                childDepartmentId: childrenDepartmentId,
                archetypeIds: new List<int>() { (int)ArchetypeEnum.Company }, 
                statusIds: statusEnums, 
                extIds: extIds, 
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items, 
                 
                pagingDto.PageIndex, 
                pagingDto.PageSize, 
                pagingDto.HasMoreData,
                selectIdentity);
        }

        /// <summary>
        /// Insert Company
        /// </summary>
        /// <param name="companyDto"></param>
        /// <returns></returns>
        [Route("companies", Name = "companies:insert")]
        [HttpPost]
        [ProducesResponseType(typeof(CompanyDto),200)]
        public IActionResult InsertCompany([FromBody]CompanyDto companyDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment((int)companyDto.ParentDepartmentId, ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            companyDto.Identity.Id = 0;
            var departmentDtoBase = (CompanyDto)_companydepartmentService.InsertDepartment(validationSpecification, companyDto);
            return StatusCode((int)HttpStatusCode.Created, departmentDtoBase);
        }

        /// <summary>
        /// Get company by id
        /// </summary>
        /// <param name="companyid"></param>
        /// <returns></returns>
        [Route("companies/{companyid}", Name = "companies:get")]
        [HttpGet]
        [ProducesResponseType(typeof(CompanyDto),200)]
        public IActionResult GetCompany(int companyid)
        {
            var companyDto = _companydepartmentService.GetDepartments<CompanyDto>(departmentIds: new List<int>() { companyid },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.Company },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            if(companyDto == null)
            {
                return CreateNotFoundResponse<CompanyDto>();
            }
            return CreateResponse<CompanyDto>(companyDto);
        }

        /// <summary>
        /// Update company
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="companyDto"></param>
        /// <returns></returns>
        [Route("companies/{companyid}", Name = "companies:update")]
        [HttpPut]
        [ProducesResponseType(typeof(CompanyDto),200)]
        public IActionResult UpdateCompany(int companyid, [FromBody]CompanyDto companyDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment((int)companyDto.ParentDepartmentId, ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .ValidateDepartment(companyid, ArchetypeEnum.Company)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            companyDto.Identity.Id = companyid;
            var dataResponse = (CompanyDto)_companydepartmentService.UpdateDepartment(validationSpecification, companyDto);
            return CreateResponse(dataResponse);

        }

        /// <summary>
        /// Get Company's candidate pools
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="statusEnums">candidate pool status enums</param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [Route("companies/{companyid}/candidatepools")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidatePoolDto>),200)]
        public IActionResult GetCandidatePoolsOnCompany(int companyid,
            [FromQuery] List<EntityStatusEnum> statusEnums=null)
        {
            
            var candidatepools = _userGroupService.GetUserGroups<CandidatePoolDto>(departmentIds: new List<int>() { companyid },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.CandidatePool },
                statusIds: statusEnums).Items;

            return base.CreateResponse(candidatepools);
        }

        /// <summary>
        /// Get candidatepool of the company by identifier
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="candidatepoolid"></param>
        /// <returns></returns>
        [Route("companies/{companyid}/candidatepools/{candidatepoolid}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidatePoolDto>),200)]
        public IActionResult GetCandidatePoolOnCompany(int companyid, 
            int candidatepoolid)
        {
            var candidatepool = _userGroupService.GetUserGroups<CandidatePoolDto>(departmentIds: new List<int>() { companyid },
                userGroupIds:new List<int>() { candidatepoolid},
                archetypeIds: new List<int>() { (int)ArchetypeEnum.CandidatePool },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All}).Items.FirstOrDefault();

            return CreateResponse<CandidatePoolDto>(candidatepool);
        }

        /// <summary>
        /// Get Company's candidates
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="statusEnums"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [Route("companies/{companyid}/candidates")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidateDto>),200)]
        public IActionResult GetCandidatesOnCompany(int companyid,
            [FromQuery] List<EntityStatusEnum> statusEnums = null)
        {
            var candidates = _userService.GetUsers<CandidateDto>(parentDepartmentIds: new List<int>() { companyid },
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Candidate },
                statusIds: statusEnums).Items;
            return base.CreateResponse(candidates);
        }

        /// <summary>
        /// Get Company's candidate
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="candidateid"></param>
        /// <returns></returns>
        [Route("companies/{companyid}/candidates/{candidateid}")]
        [HttpGet]
        [ProducesResponseType(typeof(CandidateDto),200)]
        public IActionResult GetCandidateOnCompany(int companyid, 
            int candidateid)
        {
            var candidate = _userService.GetUsers<CandidateDto>(parentDepartmentIds: new List<int>() { companyid },
                userIds:new List<int>() { candidateid},
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Candidate }, 
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All}).Items
                .FirstOrDefault();

            return CreateResponse<CandidateDto>(candidate);
        }
    }
}
