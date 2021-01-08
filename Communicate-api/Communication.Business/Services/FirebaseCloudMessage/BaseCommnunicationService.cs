using Communication.Business.Exceptions;
using Communication.Business.Extensions;
using Communication.Business.Models;
using Communication.Business.Models.Email;
using Communication.DataAccess.Notification;

using DnsClient.Internal;

using HandlebarsDotNet;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Communication.Business.Services.FirebaseCloudMessage
{
    public abstract class BaseCommnunicationService : ICommunicationService
    {
        private readonly INotificationHistoryRepository _notificationHistoryRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationPullHistoryRepository _notificationPullHistoryRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<BaseCommnunicationService> _logger;
        private readonly IUserNotificationSettingRepository _userNotificationSettingRepository;

        public BaseCommnunicationService(INotificationHistoryRepository notificationHistoryRepository, INotificationRepository notificationRepository,
            INotificationPullHistoryRepository notificationPullHistoryRepository, IWebHostEnvironment hostingEnvironment, ILogger<BaseCommnunicationService> logger,
            IUserNotificationSettingRepository userNotificationSettingRepository)
        {
            _notificationHistoryRepository = notificationHistoryRepository;
            _notificationRepository = notificationRepository;
            _notificationPullHistoryRepository = notificationPullHistoryRepository;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _userNotificationSettingRepository = userNotificationSettingRepository;
        }

        protected BaseCommnunicationService()
        {
        }

        public async Task<bool> DeleteNotificationHistory(string notificationhistoryid)
        {
            return await _notificationHistoryRepository.Update(GetDbObjectId(notificationhistoryid), new UpdateDefinitionBuilder<NotificationHistory>()
                .Set("DeletedDateUtc", DateTime.UtcNow));
        }

        public virtual async Task<(int, int, int, IEnumerable<dynamic>)> GetCommunicationHistory(string userId, int pageNo,
            int pageSize, NotificationType? notificationType = null,
            DateTime? startDate = null, DateTime? endDate = null,
            DateTime? validOn = null, bool? getUnreadMessage = null,
            bool? getActionMessage = null)
        {
            var dateToGet = new DateTime(year: 2020, month: 10, day: 17);
            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException($"UserId is required", ApplicationErrorCodes.BAD_REQUEST, System.Net.HttpStatusCode.BadRequest);
            List<NotificationHistory> histories;
            List<Notification> notifications;
            DateTime? validOnUtc = !validOn.HasValue ? default(DateTime?) : validOn.Value.ToUniversalTime();
            var globalNotificationIds = new List<ObjectId>();
            var globalNotifications = new List<Notification>();
            if (notificationType != null && notificationType.Value == NotificationType.Banner)
            {
                IMongoQueryable<Notification> globalNotificationQuery = _notificationRepository.GetCollectionQuery();
                if (startDate.HasValue)
                {
                    globalNotificationQuery = globalNotificationQuery.Where(x => x.StartDateUtc >= startDate);
                }
                if (endDate.HasValue)
                {
                    globalNotificationQuery = globalNotificationQuery.Where(x => x.EndDateUtc <= endDate);
                }
                if (validOn.HasValue)
                {
                    globalNotificationQuery = globalNotificationQuery.Where(x => x.StartDateUtc <= validOnUtc && x.EndDateUtc >= validOnUtc);
                }
                globalNotificationQuery = globalNotificationQuery.Where(x => x.NotificationType == notificationType);
                globalNotificationQuery = globalNotificationQuery.Where(x => x.Active && x.IsGlobal.Value);
                globalNotificationQuery = globalNotificationQuery.Where(x => x.CreatedDateUtc > dateToGet);
                _logger.LogInformation($"globalNotificationQuery: {globalNotificationQuery.ToString()}");
                globalNotifications = await globalNotificationQuery.ToListAsync();

                globalNotificationIds = globalNotifications.Select(x => x.Id).ToList();
            }
            var notificationHistoriesQuery = _notificationHistoryRepository.GetCollectionQuery();
            notificationHistoriesQuery = notificationHistoriesQuery.Where(x => x.UserId == userId);
            if (getUnreadMessage.HasValue)
            {
                if (getUnreadMessage.Value)
                    notificationHistoriesQuery = notificationHistoriesQuery.Where(x => x.DateReadUtc == null);
                else
                    notificationHistoriesQuery = notificationHistoriesQuery.Where(x => x.DateReadUtc != null);
            }
            if (notificationType.HasValue)
            {
                notificationHistoriesQuery = notificationHistoriesQuery.Where(x => x.NotificationType == notificationType);
            }
            notificationHistoriesQuery = notificationHistoriesQuery.Where(x => x.DeletedDateUtc == null);
            notificationHistoriesQuery = notificationHistoriesQuery.Where(x => x.CreatedDateUtc > dateToGet);
            var total = await notificationHistoriesQuery.CountAsync();
            notificationHistoriesQuery = notificationHistoriesQuery.OrderByDescending(x => x.CreatedDateUtc);
            if (pageNo > 0 && pageSize > 0)
            {
                notificationHistoriesQuery = notificationHistoriesQuery.Skip(pageSize * (pageNo - 1)).Take(pageSize);
            }
            _logger.LogInformation($"notificationHistoriesQuery: {notificationHistoriesQuery.ToString()}");
            histories = await notificationHistoriesQuery.ToListAsync();
            var totalUnread = (int)await _notificationHistoryRepository.CountAllAsync(x => x.UserId == userId && x.DeletedDateUtc == null && x.DateReadUtc == null && x.CreatedDateUtc > dateToGet);
            var totalNotPulled = 0;

            var notificationIds = histories.Where(x => !globalNotificationIds.Contains(x.NotificationId)).Select(x => x.NotificationId).Distinct().ToList();

            IMongoQueryable<Notification> notificationQuery = _notificationRepository.GetCollectionQuery();
            notificationQuery = notificationQuery.Where(x => notificationIds.Contains(x.Id));
            if (startDate.HasValue)
            {
                notificationQuery = notificationQuery.Where(x => x.StartDateUtc >= startDate);
            }
            if (endDate.HasValue)
            {
                notificationQuery = notificationQuery.Where(x => x.EndDateUtc <= endDate);
            }
            if (validOn.HasValue)
            {
                notificationQuery = notificationQuery.Where(x => x.StartDateUtc <= validOnUtc && x.EndDateUtc >= validOnUtc);
            }
            if (notificationType.HasValue)
            {
                notificationQuery = notificationQuery.Where(x => x.NotificationType == notificationType);
            }
            if (getActionMessage.HasValue)
            {
                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Exists("data.ActionUrl", getActionMessage.Value);
                notificationQuery = notificationQuery.Where(_ => filter.Inject());
            }
            notificationQuery = notificationQuery.Where(x => x.Active);
            notificationQuery = notificationQuery.Where(x => x.CreatedDateUtc > dateToGet);
            _logger.LogInformation($"notificationQuery: {notificationQuery.ToString()}");
            notifications = await notificationQuery.ToListAsync();
            notifications.AddRange(globalNotifications);
            if (notificationType != null && notificationType.Value == NotificationType.Banner)
            {
                var globalHistories = new List<NotificationHistory>();
                foreach (var item in globalNotifications)
                {
                    globalHistories.Add(new NotificationHistory
                    {
                        NotificationId = item.Id,
                        DeletedDateUtc = null,
                        DateReadUtc = DateTime.UtcNow,
                        ClientId = new HashSet<string> { item.ClientId },
                        CreatedDateUtc = item.CreatedDateUtc,
                        DepartmentId = null,
                        DepartmentTypeId = null,
                        ItemId = null,
                        NotificationType = item.NotificationType,
                        Role = null,
                        UserGroupId = null,
                        UserId = userId,
                        UserTypeId = null
                    });
                }

                var dbGlobalHistories = await _notificationHistoryRepository.GetAllAsyncPaging(0, 0, x => x.CreatedDateUtc, false, x => x.UserId == userId,
                    x => globalNotificationIds.Contains(x.NotificationId), x => x.CreatedDateUtc > dateToGet);
                foreach (var item in dbGlobalHistories)
                {
                    var match = globalHistories.FirstOrDefault(x => x.NotificationId == item.NotificationId);
                    if (match != null)
                    {
                        globalHistories.Remove(match);
                    }
                }
                histories.AddRange(globalHistories);
                histories = histories.OrderByDescending(x => x.CreatedDateUtc).ToList();
                if (globalHistories.Any())
                {
                    total += globalHistories.Count();
                    await _notificationHistoryRepository.InsertManyAsync(globalHistories);
                }
            }
            var result = new List<dynamic>();
            var lastRead = (await _notificationPullHistoryRepository.GetAllAsync(t => t.UserId == userId && t.PullingAtUtc > dateToGet)).FirstOrDefault();
            foreach (var item in histories)
            {
                dynamic historyInfo = new ExpandoObject();
                var notification = notifications.FirstOrDefault(x => x.Id == item.NotificationId);
                if (notification == null)
                    continue;
                historyInfo.MessageId = item.Id;
                historyInfo.StartDateUtc = notification.StartDateUtc;
                historyInfo.EndDateUtc = notification.EndDateUtc;
                historyInfo.NotificationId = item.NotificationId;
                historyInfo.IsGlobalMessage = notification.IsGlobal.HasValue ? notification.IsGlobal.Value : false;
                historyInfo.CreatedDateUtc = item.CreatedDateUtc;
                historyInfo.DateReadUtc = item.DateReadUtc;
                historyInfo.Subject = notification.DefaultSubject;
                historyInfo.NotificationType = item.NotificationType;
                historyInfo.Body = GetNotificationBody(notification, item.UserId);
                if (notification.Data != null)
                    historyInfo.Data = BsonTypeMapper.MapToDotNetValue(notification.Data);
                if ((lastRead != null && item.CreatedDateUtc > lastRead.PullingAtUtc) || lastRead == null)
                {
                    historyInfo.New = true;
                    totalNotPulled++;
                }
                result.Add(historyInfo);
            }
            if (notificationType != null && notificationType.Value == NotificationType.Banner)
            {
                result = result.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            }
            return (total, totalUnread, totalNotPulled, result);
        }

        public async Task<Dictionary<string, List<NotificationDigestModel>>> GetDigestNotification(DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Started getting notifications...");
            var notifications = await _notificationRepository.GetAllAsync(x => x.CreatedDateUtc >= startDate && x.CreatedDateUtc <= endDate);
            var userSettings = await _userNotificationSettingRepository.GetAllAsync(x => x.EnableDigest);
            var userIds = userSettings.Select(x => x.UserId).Distinct().ToHashSet();
            var histories = await _notificationHistoryRepository.GetAllAsyncPaging(0, 0, x => x.CreatedDateUtc, false, x => userIds.Contains(x.UserId),
                x => x.CreatedDateUtc >= startDate, x => x.CreatedDateUtc >= startDate);
            _logger.LogInformation("Finished getting notifications.");

            _logger.LogInformation("Started summary notifications...");
            var notificationHashSet = notifications.ToHashSet();
            var notificationDigestModels = new Dictionary<string, List<NotificationDigestModel>>();
            foreach (var item in histories)
            {
                var historyInfo = new NotificationDigestModel();
                var notification = notificationHashSet.FirstOrDefault(x => x.Id == item.NotificationId);
                if (notification == null)
                    continue;
                historyInfo.SentDateUtc = item.CreatedDateUtc.AddHours(8).ToString("hh:mm tt");
                historyInfo.Subject = notification.DefaultSubject;
                historyInfo.Body = GetNotificationBody(notification, item.UserId);

                if (notificationDigestModels.ContainsKey(item.UserId))
                    notificationDigestModels[item.UserId].Add(historyInfo);
                else
                    notificationDigestModels.Add(item.UserId, new List<NotificationDigestModel> { });
            }
            _logger.LogInformation($"Finished summary notifications. Aggregated {notificationDigestModels.Count} result(s)");
            return notificationDigestModels;
        }

        private string GetNotificationBody(Notification notification, string userId)
        {
            if (!string.IsNullOrEmpty(notification.DefaultPlainTextBody))
                return notification.DefaultPlainTextBody;
            if (notification.TemplateData != null)
            {
                var templateData = Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateData>(notification.TemplateData.ToJson());
                string textPath = Path.Combine(_hostingEnvironment.ContentRootPath, "NotificationTemplate", templateData.Project,
                    templateData.Module, templateData.TemplateName + ".txt");
                if (File.Exists(textPath))
                {
                    var text = File.ReadAllText(textPath);
                    var template = !string.IsNullOrEmpty(text) ? Handlebars.Compile(text) : null;
                    return template(templateData.Data);
                }
                else
                {
                    _logger.LogWarning($"Missing template path: {textPath}, notificationId {notification.Id}");
                }
            }
            if (!string.IsNullOrEmpty(notification.DefaultBody))
                return HtmlExtensions.HtmlToPlainText(notification.DefaultBody, false);
            return string.Empty;
        }

        public async Task<bool> SetCommunicationRead(string notificationId, string userId)
        {
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(notificationId))
            {
                var builder = new FilterDefinitionBuilder<NotificationHistory>();
                var filterDefinition = new FilterDefinitionBuilder<NotificationHistory>().And(builder.Eq(x => x.UserId, userId), builder.Eq(x => x.NotificationId, GetDbObjectId(notificationId)), builder.Eq(x => x.DateReadUtc, null));
                var task = await _notificationHistoryRepository.UpdateManyAsync(filterDefinition, new UpdateDefinitionBuilder<NotificationHistory>()
                                                                            .Set("DateReadUtc", DateTime.UtcNow));
                return task.IsAcknowledged;
            }
            if (!string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(notificationId))
            {
                var builder = new FilterDefinitionBuilder<NotificationHistory>();
                var filterDefinition = new FilterDefinitionBuilder<NotificationHistory>().And(builder.Eq(x => x.UserId, userId), builder.Eq(x => x.DateReadUtc, null));
                var task = await _notificationHistoryRepository.UpdateManyAsync(filterDefinition, new UpdateDefinitionBuilder<NotificationHistory>()
                                                                            .Set("DateReadUtc", DateTime.UtcNow));
                return task.IsAcknowledged;
            }
            if (!string.IsNullOrEmpty(notificationId))
            {
                var builder = new FilterDefinitionBuilder<NotificationHistory>();
                var filterDefinition = new FilterDefinitionBuilder<NotificationHistory>().And(builder.Eq(x => x.UserId, userId), builder.Eq(x => x.NotificationId, GetDbObjectId(notificationId)), builder.Eq(x => x.DateReadUtc, null));
                return await _notificationHistoryRepository.UpdateOneAsync(filterDefinition, new UpdateDefinitionBuilder<NotificationHistory>()
                                          .Set("DateReadUtc", DateTime.UtcNow));
            }
            return false;
        }
        public async Task<bool> UpdateNotification(string externalId, string clientId, NotificationUpdateModel notification)
        {
            var builder = new FilterDefinitionBuilder<Notification>();
            var filterDefinition = new FilterDefinitionBuilder<Notification>().And(builder.Eq(x => x.ClientId, clientId), builder.Eq(x => x.ExternalId, externalId));
            var result = await _notificationRepository.UpdateManyAsync(filterDefinition, new UpdateDefinitionBuilder<Notification>()
                                      .Set("Active", notification.Active)
                                      .Set("DefaultBody", notification.DefaultBody)
                                      .Set("DefaultSubject", notification.DefaultSubject)
                                      .Set("StartDateUtc", notification.StartDateUtc)
                                      .Set("EndDateUtc", notification.EndDateUtc)
                                      .Set("IsGlobal", notification.IsGlobal));
            return result.IsAcknowledged && result.ModifiedCount >= 0;
        }

        public async Task<bool> DeactiveNotification(string externalId, string clientId)
        {
            var builder = new FilterDefinitionBuilder<Notification>();
            var filterDefinition = new FilterDefinitionBuilder<Notification>().And(builder.Eq(x => x.ClientId, clientId), builder.Eq(x => x.ExternalId, externalId));
            var ressult = await _notificationRepository.UpdateManyAsync(filterDefinition, new UpdateDefinitionBuilder<Notification>()
                                      .Set("Active", false));
            return ressult.IsAcknowledged;
        }

        public async Task<List<Notification>> GetAllNotification(string clientId)
        {
            return await _notificationRepository.GetAllAsync(notification => notification.ClientId == clientId);
        }

        protected ObjectId GetDbObjectId(string id)
        {
            try
            {
                var dbId = new ObjectId(id);
                return dbId;
            }
            catch (Exception)
            {
                throw new BadRequestException($"Invalid id: {id}", ApplicationErrorCodes.BAD_REQUEST, System.Net.HttpStatusCode.BadRequest);
            }
        }

        public virtual Task CancelNotification(string userId, string itemId)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task DeleteRegisterInfo(string registrationToken)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<dynamic> RegisterCommunication(string userid, string deviceId, string platform, string instanceIdToken, string clientId)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task SendCommunicationAsync(CommunicationModelBase communicationModel)
        {
            throw new System.NotImplementedException();
        }



        public virtual Task SubscribeAsync(ISet<string> topics, ISet<string> instanceIdtokens)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task SyncSubscribeAsync(ISet<string> topics, ISet<string> userids)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task TrackNotificationPullHistory(string userId)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task UnSubscribeAsync(ISet<string> topics, ISet<string> instanceIdtokens)
        {
            throw new System.NotImplementedException();
        }


    }
}