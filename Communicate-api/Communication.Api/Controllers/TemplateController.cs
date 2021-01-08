using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Communication.Business.Models;
using Communication.Business.Services;
using Communication.Business.Services.Template;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Communication.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ICommunicationTemplateService _communicationTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CommunicationController> _logger;

        public TemplateController(ICommunicationTemplateService communicationTemplateService,
            IWebHostEnvironment hostingEnvironment, IConfiguration configuration, ILogger<CommunicationController> logger)
        {
            this._communicationTemplateService = communicationTemplateService;
            this._hostingEnvironment = hostingEnvironment;
            this._configuration = configuration;
            this._logger = logger;
        }
        [HttpGet]
        [Route("/templates")]
        public async Task<IActionResult> GetTemplates(string id, string languageCode, string tag, string name)
        {
            var data = await _communicationTemplateService.GetTemplates(id, languageCode, tag, name);
            if (data.Any())
                return Ok(data);
            return NoContent();
        }
        [HttpPost]
        [Route("/templates")]
        public async Task<IActionResult> CreateTemplate([FromBody] NotificationTempateModel model)
        {
            var result = await _communicationTemplateService.InsertTemplate(model);
            return CreatedAtAction(nameof(GetTemplates), new { id = result.Id }, result);
        }
        [HttpPost]
        [Route("/templates/{templateid}/template_languages")]
        public async Task<IActionResult> CreateTemplateLanguage(string templateid, [FromBody] NotificationTemplateLanguageModel model)
        {
            var result = await _communicationTemplateService.InsertTemplateLanguage(templateid, model);
            return CreatedAtAction(nameof(GetTemplates), new { id = result.Id }, result);
        }
        [HttpPut]
        [Route("/templates/{id}")]
        public async Task<IActionResult> UpdateTemplate(string id, [FromBody] NotificationTempateModel model)
        {
            var result = await _communicationTemplateService.UpdateTemplate(id, model);
            return Ok();
        }
        [HttpPut]
        [Route("/templates/{templateid}/template_languages/{id}")]
        public async Task<IActionResult> UpdateTemplateLanguage(string templateid, string id, [FromBody] NotificationTemplateLanguageModel model)
        {
            var result = await _communicationTemplateService.UpdateTemplateLanguage(id, templateid, model);
            return Ok();
        }
        [HttpDelete]
        [Route("/templates/{id}")]
        public async Task<IActionResult> DeleteTemplate(string id)
        {
            await _communicationTemplateService.DeleteTemplate(id);
            return Ok();
        }
        [HttpDelete]
        [Route("/templates/{templateid}/template_languages/{id}")]
        public async Task<IActionResult> DeleteTemplateLanguage(string templateid, string id)
        {
            await _communicationTemplateService.DeleteTemplateLanguage(templateid, id);
            return Ok();
        }
    }
}