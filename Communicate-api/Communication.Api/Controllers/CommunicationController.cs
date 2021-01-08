using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Communication.Business.Models;
using Communication.Business.Models.Email;
using Communication.Business.Models.UserNotificationSetting;
using Communication.Business.Services;
using Communication.Business.Services.Email;
using Communication.Business.Services.FirebaseCloudMessage;
using Communication.Business.Services.NotificationSetting;
using Communication.DataAccess.Notification;
using HtmlAgilityPack;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Communication.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("communication")]
    public class CommunicationController : ControllerBase
    {
        private readonly IEnumerable<ICommunicationService> _communicationServiceResolver;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        private readonly ILogger<CommunicationController> _logger;
        private readonly IUserNotificationSettingService _registerUserNotificationSettingService;

        public CommunicationController(IEnumerable<ICommunicationService> communicationServiceResolver,
            IWebHostEnvironment hostingEnvironment,
            IConfiguration configuration,
            ILogger<CommunicationController> logger,
            IUserNotificationSettingService registerUserNotificationSettingService)
        {
            _communicationServiceResolver = communicationServiceResolver;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _logger = logger;
            _registerUserNotificationSettingService = registerUserNotificationSettingService;
        }
        [HttpPost]
        [Route("notification/register")]
        public async Task<IActionResult> RegisterNotification([FromBody] NotificationRegisterModel model)
        {
            var communicationService = _communicationServiceResolver.FirstOrDefault(x => x.GetType() == typeof(FcmPushNotificationService));

            var document = await communicationService.RegisterCommunication(model.UserId, model.DeviceId, model.Platform, model.InstanceIdToken, model.ClientId);

            return Ok(document);
        }
        [HttpPut]
        [Route("notificationsettings")]
        public async Task<IActionResult> RegisterNotificationSetting([FromBody] UserNotificationSettingCommand model)
        {
            var res = await _registerUserNotificationSettingService.RegisterUserNotificationSetting(model);
            if (res.IsFailure)
                return BadRequest(res.Error);
            return Ok();
        }

        [HttpGet]
        [Route("notificationsettings")]
        public async Task<IActionResult> GetNotificationSetting()
        {
            var res = await _registerUserNotificationSettingService.GetUserNotificationSetting(User?.FindFirst(c => string.Equals(c.Type, "sub", StringComparison.CurrentCultureIgnoreCase))?.Value);
            if (res.IsFailure)
                return BadRequest(res.Error);
            return Ok(res.Value);
        }

        [HttpPost]
        [Route("notification/un_register/{token}")]
        public async Task<IActionResult> UnRegisterNotification(string token)
        {
            var communicationService = _communicationServiceResolver.FirstOrDefault(x => x.GetType() == typeof(FcmPushNotificationService));

            await communicationService.DeleteRegisterInfo(token);

            return Ok();
        }

        [HttpGet]
        [Route("notification/get_notification_history")]
        public async Task<IActionResult> GetNotificationHistory(string userId
            , NotificationType? notificationType = null, DateTime? startDate = null, 
            DateTime? endDate = null, DateTime? validOn = null, 
            bool? getActionMessage = null, int pageSize = 100,
            bool? getUnreadMessage = null, int pageNo = 1)
        {
            var communicationService = _communicationServiceResolver.FirstOrDefault(x => x.GetType() == typeof(FcmPushNotificationService));

            var total = 0;
            var totalUread = 0;
            var totalNotPulled = 0;
            IEnumerable<dynamic> document = null;
            (total, totalUread, totalNotPulled, document) = await communicationService.GetCommunicationHistory(userId, pageNo,
                pageSize, notificationType,
                startDate, endDate,
                validOn, getUnreadMessage,
                getActionMessage: getActionMessage);
            var responseInfo = new
            {
                TotalCount = total,
                TotalUnreadCount = totalUread,
                TotalNewCount = totalNotPulled,
                PageItems = document
            };
            return Ok(responseInfo);
        }

        [HttpPost]
        [Route("notification/track_notification_history")]
        public async Task<IActionResult> TrackNotificationHistory([FromQuery] string userId)
        {
            var communicationService = _communicationServiceResolver.FirstOrDefault(x => x.GetType() == typeof(FcmPushNotificationService));
            await communicationService.TrackNotificationPullHistory(userId);

            return Ok();
        }

        [HttpPut]
        [Route("notification/mark_as_read/{messageId}")]
        public async Task<IActionResult> MarkAsRead(string messageId)
        {
            var userId = HttpContext?.User
                ?.FindFirst(c => string.Equals(c.Type, "sub", StringComparison.CurrentCultureIgnoreCase))?.Value;
            var communicationService = _communicationServiceResolver.FirstOrDefault(x => x.GetType() == typeof(FcmPushNotificationService));
            var result = await communicationService.SetCommunicationRead(messageId, userId);
            if (result)
                return Ok();
            return NoContent();
        }

        [HttpPut]
        [Route("notification/mark_all_as_read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = HttpContext?.User
                ?.FindFirst(c => string.Equals(c.Type, "sub", StringComparison.CurrentCultureIgnoreCase))?.Value;
            var communicationService = _communicationServiceResolver.FirstOrDefault(x => x.GetType() == typeof(FcmPushNotificationService));
            await communicationService.SetCommunicationRead(null, userId);
            return Ok();
        }

        [HttpGet]
        [Route("notifications")]
        public async Task<IActionResult> GetAllNotifications(string clientId)
        {
            var communicationService = _communicationServiceResolver.FirstOrDefault(x => x.GetType() == typeof(FcmPushNotificationService));
            var notifications = await communicationService.GetAllNotification(clientId);
            if (notifications.Any())
            {
                return Ok(notifications);
            }    

            return NoContent();
        }

        [HttpPut]
        [Route("notification/deactive")]
        public async Task<IActionResult> DeactiveNotification(string externalId, string clientId)
        {
            var communicationService = _communicationServiceResolver.FirstOrDefault(x => x.GetType() == typeof(FcmPushNotificationService));
            var result = await communicationService.DeactiveNotification(externalId, clientId);
            if (result)
                return Ok();
            return NoContent();
        }

        [HttpDelete]
        [Route("notification/{messageId}")]
        public async Task<IActionResult> DeleteNotificationHistory(string messageId)
        {
            var communicationService = _communicationServiceResolver.FirstOrDefault(x => x.GetType() == typeof(FcmPushNotificationService));
            var result = await communicationService.DeleteNotificationHistory(messageId);
            return Ok();
        }

        [HttpPut]
        [Route("notification/{clientId}/{externalId}")]
        public async Task<IActionResult> UpdateNotification(string clientId, string externalId, NotificationUpdateModel notification)
        {
            var communicationService = _communicationServiceResolver.FirstOrDefault(x => x.GetType() == typeof(FcmPushNotificationService));
            var result = await communicationService.UpdateNotification(externalId, clientId, notification);
            if (result)
                return Ok();
            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("email/send_email")]
        public IActionResult SendNotification([FromBody] EmailModel model)
        {
            var communicationService = _communicationServiceResolver.FirstOrDefault(x => x.GetType() == typeof(EmailService));
            communicationService.SendCommunicationAsync(model);
            return Accepted();
        }

        [Route("extract")]
        public IActionResult SelectDivText()
        {
            var directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "EmailTemplate", "Opal");
            var files = Directory.EnumerateFiles(directoryPath, "*.html", SearchOption.AllDirectories);
            foreach(var item in files)
            {
                var fileName = Path.GetFileName(item);
                
                var doc = new HtmlDocument();
                doc.Load(item);
                var stringBuilder = new StringBuilder();
                var nodes = doc.DocumentNode.SelectNodes("//body");
                if (nodes != null)
                {
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//body"))
                    {
                        var text = Regex.Replace(node.InnerText, @"[^\S\r\n]", " ").Trim();
                        stringBuilder.Append(text);
                    }
                    var fullText = stringBuilder.ToString();
                    var result = Regex.Split(fullText, "\r\n|\r|\n").Where(x => x.Length > 0);
                    stringBuilder = new StringBuilder();
                    foreach (var rawString in result)
                    {
                        var rawStringTemp = rawString.Trim();
                        if (rawStringTemp.Length > 0)
                            stringBuilder.AppendLine(rawStringTemp);
                    }
                }
                Console.WriteLine(stringBuilder);
                try
                {
                    using (FileStream fs = System.IO.File.Create(item.Replace("EmailTemplate", "NotificationTemplate").Replace(".html", ".txt")))
                    {
                        // Add some text to file    
                        Byte[] title = new UTF8Encoding(true).GetBytes(stringBuilder.ToString());
                        fs.Write(title, 0, title.Length);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                if (System.IO.File.Exists(item.Replace("EmailTemplate", "NotificationTemplate")))
                {
                    System.IO.File.Delete(item.Replace("EmailTemplate", "NotificationTemplate"));
                }
            }

            
            return Ok();
        }

    }
}
