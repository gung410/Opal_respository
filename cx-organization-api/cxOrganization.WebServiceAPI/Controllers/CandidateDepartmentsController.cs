using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cxOrganization.WebServiceAPI.Controllers
{
    /// <summary>
    /// CandidateDepartments API 
    /// </summary>
    [Authorize]
    public class CandidateDepartmentsController : ApiControllerBase
    {
        private readonly IDepartmentService _candidateDepartmentService;
        private readonly IWorkContext _workContext;

        /// <summary>
        /// CandidateDepartments API Constructor
        /// </summary>
        /// <param name="departmentService"></param>
        /// <param name="workContext"></param>
        public CandidateDepartmentsController(Func<ArchetypeEnum, IDepartmentService> departmentService,
            IWorkContext workContext)
        {
            _candidateDepartmentService = departmentService(ArchetypeEnum.CandidateDepartment);
            _workContext = workContext;

        }
        /// <summary>
        ///  Get candidate Departments
        /// </summary>
        /// <param name="candidateDepartmentIds"></param>
        /// <param name="statusEnums">CandidateDepartment status enums: Default is active</param>
        /// <param name="selectIdentity"></param>
        /// <param name="extIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="parentDepartmentId"></param>
        /// <param name="orderBy"></param>
        /// <param name="extId">obsolete</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("candidatedepartments")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidateDepartmentDto>),200)]
        public IActionResult GetCandidateDepartments(
            int parentDepartmentId = 0,
            [FromQuery] List<int> candidateDepartmentIds = null,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
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
            var candidateDepartmentDtoPaging = _candidateDepartmentService.GetDepartments<CandidateDepartmentDto>(
                ownerId: _workContext.CurrentOwnerId,
                customerIds: new List<int> { _workContext.CurrentCustomerId },
                parentDepartmentId: parentDepartmentId,
                departmentIds: candidateDepartmentIds,
                statusIds: statusEnums,
                extIds: extIds,
                lastUpdatedBefore: lastUpdatedBefore,
                archetypeIds: new List<int>() { (int)ArchetypeEnum.CandidateDepartment },
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: orderBy,
                lastUpdatedAfter: lastUpdatedAfter);

            return base.CreatePagingResponse(candidateDepartmentDtoPaging.Items,
                
                candidateDepartmentDtoPaging.PageIndex,
                candidateDepartmentDtoPaging.PageSize,
                candidateDepartmentDtoPaging.HasMoreData,
                selectIdentity);
        }
        /// <summary>
        /// insert candidate department
        /// </summary>
        /// <param name="candidateDepartmentDto"></param>
        /// <returns></returns>
        [Route("candidatedepartments", Name = "candidatedepartments:insert_CandidateDepartment")]
        [HttpPost]
        [ProducesResponseType(typeof(CandidateDepartmentDto),200)]
        public IActionResult InsertCandidateDepartment([FromBody]CandidateDepartmentDto candidateDepartmentDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(candidateDepartmentDto.ParentDepartmentId ?? 0, ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            candidateDepartmentDto.Identity.Id = 0;
            var responseCandidateDepartmentDto = (CandidateDepartmentDto)_candidateDepartmentService.InsertDepartment(validationSpecification, candidateDepartmentDto);
            return StatusCode((int)HttpStatusCode.Created, responseCandidateDepartmentDto);
        }
        /// <summary>
        /// get candidate department by candidate department's id
        /// </summary>
        /// <param name="candidatedepartmentid"></param>
        /// <returns></returns>
        [Route("candidatedepartments/{candidatedepartmentid}")]
        [HttpGet]
        [ProducesResponseType(typeof(CandidateDepartmentDto),200)]
        public IActionResult GetCandidateDepartment(int candidatedepartmentid)
        {

            var candidateDepartment = _candidateDepartmentService.GetDepartments<CandidateDepartmentDto>(departmentIds: new List<int>() { candidatedepartmentid },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            if (candidateDepartment != null)
            {
                return CreateResponse(candidateDepartment);
            }
            return CreateNotFoundResponse<CandidateDepartmentDto>();
        }

        /// <summary>
        /// update Candidate Department
        /// </summary>
        /// <param name="candidatedepartmentid"></param>
        /// <param name="candidateDepartmentDto"></param>
        /// <returns></returns>
        [Route("candidatedepartments/{candidatedepartmentid}", Name = "candidatedepartments:Update_CandidateDepartment")]
        [HttpPut]
        [ProducesResponseType(typeof(CandidateDepartmentDto),200)]
        public IActionResult UpdateCandidateDepartment(int candidatedepartmentid, [FromBody]CandidateDepartmentDto candidateDepartmentDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment((int)candidateDepartmentDto.ParentDepartmentId, ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            candidateDepartmentDto.Identity.Id = candidatedepartmentid;
            var responsedData = (CandidateDepartmentDto)_candidateDepartmentService.UpdateDepartment(validationSpecification, candidateDepartmentDto);

            return CreateResponse(responsedData);
        }

        #region will be remove
        /// <summary>
        /// Insert Candidate Department (This endpoint is obsolete)
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="companyid"></param>
        /// <param name="candidateDepartmentDto"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/companies/{companyid}/candidatedepartments", Name = "candidatedepartments:insert_on_dataowner")]
        [HttpPost]
        [ProducesResponseType(typeof(CandidateDepartmentDto),200)]
        public IActionResult InsertCandidateDepartment(int dataownerid, int companyid, [FromBody]CandidateDepartmentDto candidateDepartmentDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
            .ValidateDepartment(companyid, ArchetypeEnum.Company)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            candidateDepartmentDto.Identity.Id = 0;
            candidateDepartmentDto.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            var departmentDtoBase = (CandidateDepartmentDto)_candidateDepartmentService.InsertDepartment(validationSpecification, candidateDepartmentDto);
            return CreateResponse(departmentDtoBase);
        }

        /// <summary>
        /// Get candidate department by id (This endpoint is obsolete)
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="companyid"></param>
        /// <param name="candidatedepartmentid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/companies/{companyid}/candidatedepartments/{candidatedepartmentid}", Name = "candidatedepartments:get")]
        [HttpGet]
        [ProducesResponseType(typeof(CandidateDepartmentDto),200)]
        public IActionResult GetCandidateDepartment(int dataownerid, int companyid, int candidatedepartmentid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
            .ValidateDepartment(companyid, ArchetypeEnum.Company)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            CandidateDepartmentDto candidateDepartmentDto = _candidateDepartmentService.GetDepartment(validationSpecification, candidatedepartmentid) as CandidateDepartmentDto;
            if (candidateDepartmentDto == null)
            {
                CreateNotFoundResponse(string.Format("Candidate Department not found: candidatedepartmentid({0})", candidatedepartmentid));
            }

            return CreateResponse(candidateDepartmentDto);
        }

        /// <summary>
        /// Get candidate departments on data owner (This endpoint is obsolete)
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="companyid"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/companies/{companyid}/candidatedepartments", Name = "candidatedepartments:findall_from_dataowner")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CandidateDepartmentDto>),200)]
        public IActionResult GetCandidateDepartments(int dataownerid, int companyid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                  .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
                  .ValidateDepartment(companyid, ArchetypeEnum.Company)
                  .WithStatus(EntityStatusEnum.All)
                  .IsDirectParent()
                  .Create();

            var department = _candidateDepartmentService.GetDepartments(validationSpecification, companyid);
            if (department != null)
            {
                return CreateResponse(department);
            }
            return CreateNotFoundResponse(string.Format("No candidate department found: companyid({0})", companyid));
        }

        /// <summary>
        /// Update candidate department by given id (This endpoint is obsolete)
        /// </summary>
        /// <param name="dataownerid"></param>
        /// <param name="companyid"></param>
        /// <param name="candidatedepartmentid"></param>
        /// <param name="candidateDepartmentDto"></param>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}/companies/{companyid}/candidatedepartments/{candidatedepartmentid}", Name = "candidatedepartments:update")]
        [HttpPut]
        [ProducesResponseType(typeof(CandidateDepartmentDto),200)]
        public IActionResult UpdateCandidateDepartment(int dataownerid, int companyid, int candidatedepartmentid, CandidateDepartmentDto candidateDepartmentDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(dataownerid, ArchetypeEnum.DataOwner)
            .ValidateDepartment(companyid, ArchetypeEnum.Company)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            candidateDepartmentDto.Identity.Id = candidatedepartmentid;
            candidateDepartmentDto.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            var dataResponse = (CandidateDepartmentDto)_candidateDepartmentService.UpdateDepartment(validationSpecification, candidateDepartmentDto);
            if (dataResponse == null)
                return CreateNotFoundResponse(string.Format("Candidate Department not found: candidatedepartmentid({0}))", candidateDepartmentDto.Identity.Id));
            return CreateResponse(dataResponse);

        }
        #endregion

    }
}
