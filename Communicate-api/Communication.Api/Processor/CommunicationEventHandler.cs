using Communication.Business.Models.Event;
using Communication.Business.Services;
using Communication.Business.Services.FirebaseCloudMessage;
using Datahub.Processor.Base.ProcessorRegister;
using Datahub.Processor.Base.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Communication.Api.Processor
{
    public class CommunicationEventHandler : ActionHandlerBase, IActionHandler
    {
        private IEnumerable<ICommunicationService> _communicationServices;
        private readonly IServiceProvider _serviceProvider;
        public CommunicationEventHandler(ILogger<CommunicationEventHandler> logger, IServiceProvider serviceProvider) : base(logger)
        {
            _serviceProvider = serviceProvider;
        }

        public override string Action => ActionConstants.AcceptAll;

        public override Func<dynamic, Task> Handler => async dynamicObject =>
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                _communicationServices = scope.ServiceProvider.GetServices<ICommunicationService>();
            }
            if (dynamicObject.Type != "event")
                return;
            var messageStr = JsonConvert.SerializeObject(dynamicObject);
            var message = JsonConvert.DeserializeObject<CommunicationEvent>(messageStr);
            await HandleCommunicationEventAsync(message);
        };

        private async Task HandleCommunicationEventAsync(CommunicationEvent communicationEvent)
        {
            var communicationService = _communicationServices.FirstOrDefault(x => x.GetType().Name == typeof(FcmPushNotificationService).Name);
            if (communicationEvent.Routing.Action.Contains("communication.register.firebase.success"))
            {
                await communicationService.RegisterCommunication(communicationEvent.Payload.Body.UserId, communicationEvent.Payload.Body.DeviceId, communicationEvent.Payload.Body.Platform, communicationEvent.Payload.Body.RegistrationToken, "");
            }
            if (communicationEvent.Routing.Action.Contains("communication.logout.firebase.requested"))
            {
                await communicationService.DeleteRegisterInfo(communicationEvent.Payload.Body.RegistrationToken);
            }
            if (communicationEvent.Routing.Action.Contains("communication.mark.notification.read"))
            {
                await communicationService.SetCommunicationRead(communicationEvent.Payload.Body.NotificationId, communicationEvent.Payload.Body.UserId);
            }
            if (communicationEvent.Routing.Action.Contains("communication.cancel.notification.cancelled"))
            {
                await communicationService.CancelNotification(communicationEvent.Payload.Body.UserId, communicationEvent.Payload.Body.ItemId);
            }
        }


    }
}
