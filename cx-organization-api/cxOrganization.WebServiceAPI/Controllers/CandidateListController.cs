using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using cxOrganization.Business.CandidateList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Authorize]
    [Route("candidatelist")]
    public class CandidateListController : ApiControllerBase
    {
        private readonly ICandidateListService _candidateListService;

        public CandidateListController(ICandidateListService candidateListService)
        {
            _candidateListService = candidateListService;

        }

        /// <summary>
        /// Get candidate list
        /// </summary>
        /// <param name="candidateListArguments">The arguments for filtering, sorting, paging when retrieving candidate list</param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [ProducesResponseType(typeof(List<CandidateListItem>), 200)]
        public IActionResult GetCandidateList([Required][FromBody] CandidateListArguments candidateListArguments)
        {             
            var candidateListDto = _candidateListService.GetCandidateList(
                candidateListArguments: candidateListArguments);

            return Ok(candidateListDto);
        }
    }
}
