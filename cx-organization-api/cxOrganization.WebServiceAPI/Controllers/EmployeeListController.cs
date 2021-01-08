using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Business.PDPlanner.EmployeeList;
using cxOrganization.Domain.Extensions;
using cxPlatform.Core;
using cxPlatform.Core.Extentions.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Route("employeeList")]
    [Authorize]
    public class EmployeeListController : ApiControllerBase
    {
        private readonly IEmployeeListService _employeeListService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        public EmployeeListController(ILogger<EmployeeListController> logger, IWorkContext workContext, IEmployeeListService employeeListService)
        {
            _employeeListService = employeeListService;
            _workContext = workContext;
            _logger = logger;
        }

      
        /// <summary>
        /// Get employeeList
        /// </summary>
        /// <param name="idpEmployeeListArguments">The arguments for filtering, sorting, paging when retrieving idp Employee list</param>
        /// <returns></returns>
        [HttpPost]//route start with / will overwrite route prefix
        [ProducesResponseType(StatusCodes.Status200OK)]
        public  async Task<ActionResult<IdpEmployeeListDto>> GetIdpEmployeeList([FromBody][Required] IdpEmployeeListArguments idpEmployeeListArguments)
        {
            if (!ValidateMinimalFilter(idpEmployeeListArguments))
            {
                return Ok(new IdpEmployeeListDto
                {
                    Items = new List<IdpEmployeeItemDto>(),
                    TotalItems = 0,
                    PageSize = idpEmployeeListArguments?.GetPageSize() ?? 0,
                    PageIndex = idpEmployeeListArguments?.GetPageIndex() ?? 0,
                    HasMoreData = false
                });
            }
            var employeeListDto = await this._employeeListService.GetIdpEmployeeListAsync(idpEmployeeListArguments);
           
            return Ok(employeeListDto);
        }
  
        private bool ValidateMinimalFilter(IdpEmployeeListArguments idpEmployeeListArguments)
        {
            //Only validate when authorized by user token
            if (!string.IsNullOrEmpty(_workContext.Sub))
            {
                if (idpEmployeeListArguments == null) return false;

                var hasFilterOnUserIdentity =
                    idpEmployeeListArguments.ForCurrentUser || !idpEmployeeListArguments.UserIds.IsNullOrEmpty()
                                                            || !string.IsNullOrEmpty(idpEmployeeListArguments
                                                                .IdpEmployeeSearchKey);

                var hasFilterOnDepartment = !idpEmployeeListArguments.DepartmentIds.IsNullOrEmpty();
                var hasFilterOnUserGroup = (!idpEmployeeListArguments.MultiUserGroupIds.IsNullOrEmpty() &&
                                               idpEmployeeListArguments.MultiUserGroupIds.Any(g => !g.IsNullOrEmpty()));

                if (!hasFilterOnUserIdentity && !hasFilterOnDepartment && !hasFilterOnUserGroup)
                {
                    _logger.LogWarning(
                        "For security reason, it requires minimal filter on identity of user, department or user group to be able to retrieve employee list.");
                    return false;
                }
            }

            return true;
        }
    }
}