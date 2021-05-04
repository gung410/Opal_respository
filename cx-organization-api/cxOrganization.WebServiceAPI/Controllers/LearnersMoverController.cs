using System;
using System.Collections.Generic;
using System.Net;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Authorize]
    public class LearnersMoverController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAdvancedWorkContext _workContext;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="workContext"></param>
        /// <param name="departmentService"></param>
        public LearnersMoverController(Func<ArchetypeEnum, IUserService> userService,
            IAdvancedWorkContext workContext,
            Func<ArchetypeEnum, IDepartmentService> departmentService)
        {
            _userService = userService(ArchetypeEnum.Learner);
            _workContext = workContext;
        }

        /// <summary>
        /// Move learners to class
        /// </summary>
        /// <param name="classid"></param>
        /// <param name="learnerIds"></param>
        /// <returns></returns>
        [Route("learnersmover/movetoclass/{classid}", Name = "learnersmover:movetoclass")]
        [HttpPost]
        public IActionResult MoveLearnersToClass(int classid, List<int> learnerIds)
        {
            var newStudentUserTypeId = 54;// _applicationService.GetSiteParameterValueByKey(SiteParameters.Platform.NewSchoolYear.FresherUserType).ToInt();
            _userService.MoveLearnersToClass(classid, learnerIds, newStudentUserTypeId);

            return StatusCode((int)HttpStatusCode.OK);
        }
    }
}
