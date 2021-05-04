using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using cxOrganization.Client.Departments;
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
    /// Countries API 
    /// </summary>
    [Authorize]
    public class CountriesController : ApiControllerBase
    {
        private readonly IDepartmentService _countrydepartmentService;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IUserService _userService;

        /// <summary>
        /// Countries API Constructor
        /// </summary>
        /// <param name="departmentService"></param>
        /// <param name="workContext"></param>
        /// <param name="userService"></param>
        public CountriesController(Func<ArchetypeEnum, IDepartmentService> departmentService,
            IAdvancedWorkContext workContext,
            Func<ArchetypeEnum, IUserService> userService)
        {
            _countrydepartmentService = departmentService(ArchetypeEnum.Country);
            _workContext = workContext;
            _userService = userService(ArchetypeEnum.Candidate);

        }
        /// <summary>
        /// Get countries
        /// </summary>
        /// <param name="countryIds"></param>
        /// <param name="statusEnums"></param>
        /// <param name="parentDepartmentId"></param>
        /// <param name="childrenDepartmentId"></param>
        /// <param name="extIds"></param>
        /// <param name="extId">Will be removed</param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selectIdentity"></param>
        /// <returns></returns>
        [Route("countries", Name = "countries:findall")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CountryDto>),200)]
        public IActionResult GetCountries(
            int parentDepartmentId=0,
            int childrenDepartmentId=0,
            [FromQuery] List<int> countryIds=null,
            [FromQuery] List<EntityStatusEnum> statusEnums=null,
            [FromQuery] List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            string extId = "",
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
            var pagingDto = _countrydepartmentService.GetDepartments<CountryDto>(departmentIds: countryIds,
                parentDepartmentId : parentDepartmentId,
                childDepartmentId : childrenDepartmentId,
                statusIds: statusEnums,
                archetypeIds: new List<int>() { (int)ArchetypeEnum.Country },
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
        /// Insert Country
        /// </summary>
        /// <param name="countryDto"></param>
        /// <returns></returns>
        [Route("countries", Name = "countries:insert")]
        [HttpPost]
        [ProducesResponseType(typeof(CountryDto),200)]
        public IActionResult InsertCountry([FromBody]CountryDto countryDto)
        {
            //checking ParentDepartmentId
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment((int)countryDto.ParentDepartmentId, ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            countryDto.Identity.Id = 0;
            var departmentDtoBase = (CountryDto)_countrydepartmentService.InsertDepartment(validationSpecification, countryDto);
            return StatusCode((int)HttpStatusCode.Created, departmentDtoBase);
        }

        /// <summary>
        /// Get country
        /// </summary>
        /// <param name="countryid"></param>
        /// <returns></returns>
        [Route("countries/{countryid}", Name = "countries:get")]
        [HttpGet]
        [ProducesResponseType(typeof(CountryDto),200)]
        public IActionResult GetCountry(int countryid)
        {
            var countryDto = _countrydepartmentService.GetDepartments<CountryDto>(departmentIds:new List<int>() { countryid},
                statusIds:new List<EntityStatusEnum> { EntityStatusEnum.All}).Items.FirstOrDefault();
            if (countryDto == null)
                return CreateNotFoundResponse<CountryDto>();
            return CreateResponse<CountryDto>(countryDto);
        }

        /// <summary>
        /// Update country
        /// </summary>
        /// <param name="countryid"></param>
        /// <param name="countryDto"></param>
        /// <returns></returns>
        [Route("countries/{countryid}", Name = "countries:update")]
        [HttpPut]
        [ProducesResponseType(typeof(CountryDto),200)]
        public IActionResult UpdateCountry(int countryid, [FromBody]CountryDto countryDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment((int)countryDto.ParentDepartmentId, ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .ValidateDepartment(countryid, ArchetypeEnum.Country)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();

            countryDto.Identity.Id = countryid;
            var dataResponse = (CountryDto)_countrydepartmentService.UpdateDepartment(validationSpecification, countryDto);

            return CreateResponse(dataResponse);

        }
        /// <summary>
        /// get country's candidates
        /// </summary>
        /// <param name="countryid"></param>
        /// <param name="statusEnums">candidate status enums</param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [Route("countries/{countryid}/candidates")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidateDto>),200)]
        public IActionResult GetCandidatesOnCountry(int countryid,
            [FromQuery] List<EntityStatusEnum> statusEnums = null)
        {
            var candidates = _userService.GetUsers<CandidateDto>(archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Candidate },
                parentDepartmentIds: new List<int>() { countryid },
                statusIds: statusEnums).Items;

            return base.CreateResponse(candidates);
        }
        /// <summary>
        /// get country's candidates
        /// </summary>
        /// <param name="countryid"></param>
        /// <param name="candidateid"></param>
        /// <returns></returns>
        [Route("countries/{countryid}/candidates/{candidateid}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidateDto>),200)]
        public IActionResult GetCandidateOnCountry(int countryid, int candidateid)
        {
            var candidate = _userService.GetUsers<CandidateDto>(archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Candidate },
                parentDepartmentIds: new List<int>() { countryid },
                userIds:new List<int>() { candidateid}, 
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All}).Items.FirstOrDefault();
            
            return base.CreateResponse<CandidateDto>(candidate);
        }


    }
}
