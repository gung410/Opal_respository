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
    /// Data Owner API controller 
    /// </summary>
    [Authorize]
    public class DataOwnersController : ApiControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IWorkContext _workContext;
        private readonly IHierarchyDepartmentService _hierarchyDepartmentService;

        /// <summary>
        /// Data Owner controller api
        /// </summary>
        /// <param name="departmentService"></param>
        /// <param name="workContext"></param>
        /// <param name="hierarchyDepartmentService"></param>
        public DataOwnersController(Func<ArchetypeEnum, IDepartmentService> departmentService,
            IWorkContext workContext, IHierarchyDepartmentService hierarchyDepartmentService)
        {
            _departmentService = departmentService(ArchetypeEnum.DataOwner);
            _workContext = workContext;
            _hierarchyDepartmentService = hierarchyDepartmentService;
        }
        /// <summary>
        /// Get Data Owners
        /// </summary>
        /// <param name="dataownerIds"></param>
        /// <param name="parentDepartmentId"></param>
        /// <param name="statusEnums">DataOwner's Status : default is Active</param>
        /// <param name="extId">Will be removed</param>
        /// <param name="extIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selectIdentity"></param>
        /// <returns></returns>
        [Route("dataowners", Name = "dataowners:findall")]
        [HttpGet]
        [ProducesResponseType(typeof(List<DataOwnerDto>),200)]
        public IActionResult GetDataOwners(int parentDepartmentId=0,
            [FromQuery] List<int> dataownerIds=null,
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
            //TODO remove when system api fixed package go to production
            if (!string.IsNullOrEmpty(extId))
            {
                extIds = extIds ?? new List<string>();
                extIds.Add(extId);
            }
            var pagingDto = _departmentService.GetDepartments<DataOwnerDto>(ownerId: _workContext.CurrentOwnerId,
                parentDepartmentId: parentDepartmentId,
                customerIds: new List<int> { _workContext.CurrentCustomerId },
                departmentIds: dataownerIds,
                statusIds: statusEnums,
                archetypeIds: new List<int>() { (int)ArchetypeEnum.DataOwner },
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
        /// Insert Data Owner
        /// </summary>
        /// <param name="dataOwnerDto"></param>
        /// <returns></returns>
        [Route("dataowners", Name ="dataowner:insert")]
        [HttpPost]
        [ProducesResponseType(typeof(DataOwnerDto),200)]
        public IActionResult InsertDataOwner([FromBody]DataOwnerDto dataOwnerDto)
        {
            dataOwnerDto.Identity.Id = 0;
            //Set default parent department where the new data owner belong to
            dataOwnerDto.ParentDepartmentId = dataOwnerDto.ParentDepartmentId > 0 ? dataOwnerDto.ParentDepartmentId : 1;
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(dataOwnerDto.ParentDepartmentId ?? 0, ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            DataOwnerDto departmentDtoBase = (DataOwnerDto)_departmentService.InsertDepartment(validationSpecification, dataOwnerDto);
            departmentDtoBase.ParentDepartmentId = dataOwnerDto.ParentDepartmentId;
            return StatusCode((int)HttpStatusCode.Created, departmentDtoBase);
        }
        /// <summary>
        /// Get Data Owner
        /// </summary>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}", Name = "dataowners:get")]
        [HttpGet]
        [ProducesResponseType(typeof(DataOwnerDto),200)]
        public IActionResult GetDataOnwer(int dataownerid)
        {
            var department = _departmentService.GetDepartments<DataOwnerDto>(departmentIds: new List<int>() { dataownerid},
                statusIds:new List<EntityStatusEnum> { EntityStatusEnum.All}).Items.FirstOrDefault();
            if (department == null)
                return CreateNotFoundResponse<DataOwnerDto>();
            return CreateResponse<DataOwnerDto>(department);
           
        }
        /// <summary>
        /// Get Data Owners
        /// </summary>
        /// <returns></returns>
        [Route("dataowners/{dataownerid}", Name = "dataowners:update")]
        [HttpPut]
        [ProducesResponseType(typeof(DataOwnerDto),200)]
        public IActionResult UpdateDataOwner(int dataownerid, [FromBody]DataOwnerDto dataOwnerDto)
        {
            //Set default parent department where the new data owner belong to
            dataOwnerDto.ParentDepartmentId = dataOwnerDto.ParentDepartmentId > 0 ? dataOwnerDto.ParentDepartmentId : 1;

            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment((dataOwnerDto.ParentDepartmentId ?? 1), ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            dataOwnerDto.Identity.Id = dataownerid;
            dataOwnerDto.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            var dataResponse = _departmentService.UpdateDepartment(validationSpecification,dataOwnerDto);
            return CreateResponse(dataResponse);
        }
    }
}
