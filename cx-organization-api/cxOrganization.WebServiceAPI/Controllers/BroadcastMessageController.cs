using cxOrganization.Domain.Dtos.BroadcastMessage;
using cxOrganization.Domain.Services;
using cxPlatform.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [ApiController]
    [Route("broadcast-messages")]
    public class BroadcastMessageController : ApiControllerBase
    {
        private readonly IBroadcastMessageService _broadcastMessageService;
        private readonly IWorkContext _workContext;

        public BroadcastMessageController(IBroadcastMessageService broadcastMessageService, IWorkContext workContext)
        {
            _broadcastMessageService = broadcastMessageService;
            _workContext = workContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetBroadcastMessagesAsync([FromQuery] BroadcastMessageSearchRequest request)
        {
            var result = await _broadcastMessageService.GetBroadcastMessagesAsync(request);

            return Ok(result);
        }

        [Route("{id}")]
        [HttpGet]
        [ActionName("GetByIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBroadcastMessageByIdAsync(int id)
        {
            var broadcastMessage = await _broadcastMessageService.GetBroadcastMessageByIdAsync(id);

            if (broadcastMessage is null)
            {
                return NotFound();
            }

            return Ok(broadcastMessage);
        }

        [Route("{id}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBroadcastMessageAsync(int id, [FromBody] BroadcastMessageDto broadcastMessageUpdateDto)
        {
            if (broadcastMessageUpdateDto.BroadcastMessageId is null || id != broadcastMessageUpdateDto.BroadcastMessageId)
            {
                return BadRequest();
            }

            var updatedBroadcastMessage = await _broadcastMessageService.UpdateBroadcastMessageAsync(broadcastMessageUpdateDto, _workContext);
            if (updatedBroadcastMessage is null)
            {
                return NotFound();
            }

            return Ok(updatedBroadcastMessage);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBroadcastMessageAsync([FromBody] BroadcastMessageCreationDto broadcastMessageDto)
        {
            var createdBroadcastMessage = await _broadcastMessageService.CreateBroadcastMessageAsync(broadcastMessageDto, _workContext);

            if (createdBroadcastMessage is null)
            {
                return BadRequest();
            }

            return CreatedAtAction("GetByIdAsync", new { id = createdBroadcastMessage.BroadcastMessageId }, createdBroadcastMessage);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBroadcastMessageAsync(int id)
        {
            var deletedBroadcastMessage = await _broadcastMessageService.DeleteBroadcastMessageAsync(id);

            if (deletedBroadcastMessage is null)
            {
                return NotFound();
            }

            return Ok(deletedBroadcastMessage);
        }

        [Route("{id}")]
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeBroadcastMessageStatusAsync(int id, [FromBody] BroadcastMessageChangeStatusDto broadcastMessageChangeStatusDto)
        {
            if (id != broadcastMessageChangeStatusDto.BroadcastMessageId)
            {
                return BadRequest();
            }
            var changeResult = await _broadcastMessageService.ChangeBroadcastMessageStatusAsync(broadcastMessageChangeStatusDto, _workContext);

            if (changeResult is null)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}