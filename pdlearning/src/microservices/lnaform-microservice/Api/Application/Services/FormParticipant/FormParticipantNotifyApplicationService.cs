using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.LnaForm.Application.Events;
using Microservice.LnaForm.Application.Events.EventPayloads;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Services
{
    public class FormParticipantNotifyApplicationService : BaseApplicationService
    {
        private readonly WebAppLinkBuilder _webAppLinkBuilder;

        public FormParticipantNotifyApplicationService(
            WebAppLinkBuilder webAppLinkBuilder,
            IThunderCqrs thunderCqrs) : base(thunderCqrs)
        {
            _webAppLinkBuilder = webAppLinkBuilder;
        }

        public async Task NotifyAssignedFormParticipant(NotifyFormParticipantModel model)
        {
            await PerformSendNotify(model, FormParticipantNotifyType.Assign);
        }

        public async Task NotifyReminderedFormParticipant(NotifyFormParticipantModel model)
        {
            await PerformSendNotify(model, FormParticipantNotifyType.Remind);
        }

        public async Task NotifyRemovedFormParticipant(NotifyFormParticipantModel model)
        {
            await PerformSendNotify(model, FormParticipantNotifyType.Remove);
        }

        private async Task PerformSendNotify(NotifyFormParticipantModel model, FormParticipantNotifyType notifyType)
        {
            string templateName = string.Empty;
            string subject = string.Empty;

            switch (notifyType)
            {
                case FormParticipantNotifyType.Assign:
                    templateName = "LnaFormParticipantAssign";
                    subject = "OPAL2.0 - New survey assigned";
                    break;
                case FormParticipantNotifyType.Remind:
                    templateName = "LnaFormParticipantRemind";
                    subject = "OPAL2.0 - Survey reminder";
                    break;
                case FormParticipantNotifyType.Remove:
                    templateName = "LnaFormParticipantRemove";
                    subject = "OPAL2.0 - Survey removal";
                    break;
                default:
                    break;
            }

            var payload = new NotifyFormParticipantPayload
            {
                FormDetailUrl = _webAppLinkBuilder.GetStandaloneFormPlayerLink(model.FormOriginalObjectId),
                CreatorName = model.FormOwnerName,
                FormName = model.FormTitle,
                UserName = model.ParticipantName
            };

            var reminderByConditions = new ReminderByDto
            {
                Type = ReminderByType.AbsoluteDateTimeUTC,

                // Add 2 minutes to ensure the time is valid after the message was sent to Communication.
                Value = Clock.Now.AddMinutes(2).ToString("MM/dd/yyyy HH:mm:ss")
            };

            await ThunderCqrs.SendEvent(
                new FormParticipantNotifyEvent(
                    payload: payload,
                    participantId: model.ParcitipantId,
                    objectId: model.FormOriginalObjectId,
                    reminderByConditions: reminderByConditions,
                    template: templateName,
                    subject: subject));
        }
    }
}
