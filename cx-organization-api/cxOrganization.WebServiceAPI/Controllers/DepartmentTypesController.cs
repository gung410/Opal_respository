using cxOrganization.Client.DepartmentTypes;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Authorize]
    public class DepartmentTypesController : ApiControllerBase
    {
        private readonly IDepartmentTypeService _departmentTypeService;
        private readonly IWorkContext _workContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="departmentTypeService"></param>
        /// <param name="workContext"></param>
        public DepartmentTypesController(IDepartmentTypeService departmentTypeService,
            IWorkContext workContext)
        {
            _departmentTypeService = departmentTypeService;
            _workContext = workContext;
        }

        [Route("departmenttypes")]
        [HttpGet]
        [ProducesResponseType(typeof(List<DepartmentTypeDto>), 200)]
        public async Task<IActionResult> GetDepartmentTypes(
            [FromQuery] List<ArchetypeEnum> archetypeEnums,
            [FromQuery] List<int> departmentids = null,
            [FromQuery] List<int> departmenttypeids = null,
            [FromQuery] List<string> extIds = null,
            [FromQuery] bool includeLocalizedData = true)
        {
            var acceptLanguage = HttpContext.Request.GetTypedHeaders().AcceptLanguage;

            string langCode = acceptLanguage != null ? acceptLanguage.FirstOrDefault().Value.Value : "en-US";

            var departmentTypes = await _departmentTypeService.GetAllDepartmentTypesAsync(extIds: extIds,
                archetypeIds: archetypeEnums,
                departmentIds: departmentids,
                departmentTypeIds: departmenttypeids,
                ownerId: _workContext.CurrentOwnerId,
                includeLocalizedData: includeLocalizedData,
                langCode: langCode);

            return CreateResponse(departmentTypes);
        }

    }
}
