using Communication.Business.Exceptions;
using Communication.Business.Extensions;
using Communication.Business.HttpClients;
using Communication.Business.Models;
using Communication.Business.Models.Email;
using Communication.Business.Models.FirebaseCloudMessage;
using Communication.Business.Services.Mapping;
using Communication.DataAccess.Notification;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Communication.Business.Services.FirebaseCloudMessage
{
    public class FcmPushNotificationService : BaseCommnunicationService
    {
        private readonly IFirebaseCloudMessageHttpClient _firebaseCloudMessageHttpClient;
        private readonly IGoogleAppInstanceServerHttpClient _googleAppInstanceServerHttpClient;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationHistoryRepository _notificationHistoryRepository;
        private readonly INotificationReferenceRepository _notificationReferenceRepository;
        private readonly INotificationSubcriptionRepository _notificationSubcriptionRepository;
        private readonly IMappingService _mapper;
        private readonly ILogger<FcmPushNotificationService> _logger;
        private readonly IUserNotificationSettingRepository _userNotificationSettingRepository;
        private readonly INotificationPullHistoryRepository _notificationPullHistoryRepository;

        public FcmPushNotificationService(IFirebaseCloudMessageHttpClient firebaseCloudMessageHttpClient,
            IGoogleAppInstanceServerHttpClient googleAppInstanceServerHttpClient, IConfiguration configuration,
            IMappingService mapper,
            IWebHostEnvironment hostingEnvironment,
            INotificationRepository notificationRepository,
            INotificationHistoryRepository notificationHistoryRepository,
            INotificationReferenceRepository notificationReferenceRepository,
            INotificationSubcriptionRepository notificationSubcriptionRepository,
            INotificationPullHistoryRepository notificationPullHistoryRepository, ILogger<FcmPushNotificationService> logger, 
            IUserNotificationSettingRepository userNotificationSettingRepository) : 
            base(notificationHistoryRepository, notificationRepository, notificationPullHistoryRepository, hostingEnvironment, logger, userNotificationSettingRepository)
        {
            _firebaseCloudMessageHttpClient = firebaseCloudMessageHttpClient;
            _googleAppInstanceServerHttpClient = googleAppInstanceServerHttpClient;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _notificationRepository = notificationRepository;
            _notificationHistoryRepository = notificationHistoryRepository;
            _notificationReferenceRepository = notificationReferenceRepository;
            _notificationSubcriptionRepository = notificationSubcriptionRepository;
            _mapper = mapper;
            _notificationPullHistoryRepository = notificationPullHistoryRepository;
            _logger = logger;
            _userNotificationSettingRepository = userNotificationSettingRepository;
        }
        public override async Task SubscribeAsync(ISet<string> topics, ISet<string> instanceIdtokens)
        {
            await Task.WhenAll(ExecuteDelegateFuncAsync(topics, instanceIdtokens, _googleAppInstanceServerHttpClient.AddRelationshipMapAsync));

        }

        private async Task<ISet<string>> GetInstanceIdTokens(ISet<string> userIds)
        {
            var data = await _notificationReferenceRepository.GetAllAsync(x => userIds.Contains(x.UserId));
            return data.Select(x => x.InstanceIdToken).ToHashSet();
        }

        private async Task ExecuteDelegateFuncAsync(ISet<string> topicsToExecute,
            ISet<string> registrationDeviceTokens,
            Func<ISet<string>, string, Task> func)
        {
            if (topicsToExecute.Any())
            {
                List<Task> unsubcribeTopicsTasks = new List<Task>();
                foreach (var topic in topicsToExecute)
                {
                    unsubcribeTopicsTasks.Add(func(registrationDeviceTokens, topic));
                }
                await Task.WhenAll(unsubcribeTopicsTasks);
            }
        }


        public override async Task SendCommunicationAsync(CommunicationModelBase notificationModel)
        {
            var pushNotificationModel = notificationModel as PushNotificationModel;
            //var notificationDb = await InsertNotificationAsync(pushNotificationModel);
            FirebaseNotificationMessageBuilder notification = FirebaseNotificationMessageBuilder.GetNotificationMessageBuilder().WithData(pushNotificationModel.Data)
                                 .WithTitleAndBody(notificationModel.Subject, notificationModel.Body);
            List<Task> sendTasks = new List<Task>();
            foreach (var topic in pushNotificationModel.TopicNames)
            {
                notification = notification.ToTopic(topic);
                sendTasks.Add(_firebaseCloudMessageHttpClient.SendNotificationAsync(notification.Build()));
            }
            await Task.WhenAll(sendTasks);

        }

        public override async Task UnSubscribeAsync(ISet<string> topics, ISet<string> instanceIdtokens)
        {
            await Task.WhenAll(ExecuteDelegateFuncAsync(topics, instanceIdtokens, _googleAppInstanceServerHttpClient.RemoveRelationshipMapAsync));

        }

        public override async Task SyncSubscribeAsync(ISet<string> topics, ISet<string> userIds)
        {
            ISet<string> instanceIdtokens = await GetInstanceIdTokens(userIds);
            foreach (var item in instanceIdtokens)
            {
                var currentRegistrationTopics = await _googleAppInstanceServerHttpClient.GetAppInstanceRelationsAsync(item);

                // Just add the given registration token to the unsubcribed topics 
                var subscribeToTopics = topics.Where(t => !currentRegistrationTopics.Any(j => j == t)).ToHashSet();

                // Why need to unsubcribe? Because the user might sign in under another account.
                // push notification to the original account don't get sent to this device.
                var unsubcribeToTopics = currentRegistrationTopics.Where(t => !topics.Any(j => j == t)).ToHashSet();

                await Task.WhenAll(ExecuteDelegateFuncAsync(subscribeToTopics, new HashSet<string> { item }, _googleAppInstanceServerHttpClient.AddRelationshipMapAsync),
                                   ExecuteDelegateFuncAsync(unsubcribeToTopics, new HashSet<string> { item }, _googleAppInstanceServerHttpClient.RemoveRelationshipMapAsync));
            }
        }

        public override async Task<dynamic> RegisterCommunication(string userid, string deviceId, string platform, string instanceIdToken, string clientId)
        {
            await _googleAppInstanceServerHttpClient.ValidateToken(instanceIdToken);
            ////Store info in database
            var document = new NotificationUserInfo();
            document.UserId = userid;
            document.DeviceId = deviceId;
            document.InstanceIdToken = instanceIdToken;
            document.Platform = platform;
            document.ClientId = clientId;
            document.CreatedDateUtc = DateTime.UtcNow;
            document.Subscription = true;
            var existInfo = (await _notificationReferenceRepository.GetAllAsync(x => x.UserId == userid, x => x.DeviceId == deviceId, x => x.InstanceIdToken == instanceIdToken, x => x.ClientId == clientId)).FirstOrDefault();
            if (existInfo != null)
            {
                return document;
            }
            await _notificationReferenceRepository.InsertOneAsync(document);
            return document;
        }

        public override async Task TrackNotificationPullHistory(string userId)
        {
            await _notificationPullHistoryRepository.ReplaceOneAsync(new NotificationPullHistory { PullingAtUtc = DateTime.UtcNow, UserId = userId });
        }

        private async Task<DataAccess.Notification.Notification> InsertNotificationAsync(PushNotificationModel notificationModel)
        {
            var document = _mapper.ToNotificationEntity(notificationModel);
            await _notificationRepository.InsertOneAsync(document);
            return document;
        }

        public override async Task DeleteRegisterInfo(string registrationToken)
        {
            var filter = new FilterDefinitionBuilder<NotificationUserInfo>().Eq(x => x.InstanceIdToken, registrationToken);
            await _notificationReferenceRepository.DeleteByFilter(filter);
        }

        public override async Task CancelNotification(string userId, string itemId)
        {
            var builder = new FilterDefinitionBuilder<NotificationHistory>();
            var filterDefinition = new FilterDefinitionBuilder<NotificationHistory>()
                .And(builder.Eq(x => x.UserId, userId), builder.Eq(x => x.ItemId, itemId));
            await _notificationHistoryRepository.UpdateManyAsync(filterDefinition,
                                      new UpdateDefinitionBuilder<NotificationHistory>()
                                      .Set("Cancelled", true).Set("CancelledDateUtc", DateTime.UtcNow));
        }
    }
}
