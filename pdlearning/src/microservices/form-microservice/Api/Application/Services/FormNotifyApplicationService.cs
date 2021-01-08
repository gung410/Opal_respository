using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Form.Application.Events;
using Microservice.Form.Application.Events.EventPayloads;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.Constants;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Services
{
    public class FormNotifyApplicationService : BaseApplicationService
    {
        private readonly WebAppLinkBuilder _webAppLinkBuilder;

        public FormNotifyApplicationService(
            WebAppLinkBuilder webAppLinkBuilder,
            IThunderCqrs thunderCqrs) : base(thunderCqrs)
        {
            _webAppLinkBuilder = webAppLinkBuilder;
        }

        public async Task NotifyAssignedFormParticipant(NotifyFormParticipantModel model)
        {
            await PerformSendNotifyStandanloneParticipant(model, FormParticipantNotifyType.Assign);
        }

        public async Task NotifyReminderedFormParticipant(NotifyFormParticipantModel model)
        {
            await PerformSendNotifyStandanloneParticipant(model, FormParticipantNotifyType.Remind);
        }

        public async Task NotifyRemovedFormParticipant(NotifyFormParticipantModel model)
        {
            await PerformSendNotifyStandanloneParticipant(model, FormParticipantNotifyType.Remove);
        }

        public async Task PerformSendNotifyFormDueDate(NotifyFormDueDateModel model)
        {
            var formDetailUrl = _webAppLinkBuilder.GetFormDetailLink(model.FormID);

            var payload = new NotifyFormDueDatePayload
            {
                FormName = model.FormName,
                UserName = model.UserName,
                ActionName = model.FormName,
                RemindBeforeDays = model.ReminderBeforeDays,
                ActionUrl = formDetailUrl,
                FormDetailUrl = formDetailUrl
            };

            var reminderByConditions = new ReminderByDto
            {
                Type = ReminderByType.AbsoluteDateTimeUTC,
                Value = Clock.Now.AddMinutes(2).ToString("MM/dd/yyyy HH:mm:ss")
            };

            await ThunderCqrs.SendEvent(
                new FormDueDateNotifyEvent(payload: payload, reminderByConditions: reminderByConditions, model.FormID, model.UserId));
        }

        public async Task PerformSendNotifySurveyEndDate(NotifyFormDueDateModel model)
        {
            var formDetailUrl = _webAppLinkBuilder.GetStandaloneFormPlayerLink(model.FormID);

            var payload = new NotifyFormDueDatePayload
            {
                FormName = model.FormName,
                UserName = model.UserName,
                ActionName = model.FormName,
                RemindBeforeDays = model.ReminderBeforeDays,
                ActionUrl = formDetailUrl,
                FormDetailUrl = formDetailUrl
            };

            var reminderByConditions = new ReminderByDto
            {
                Type = ReminderByType.AbsoluteDateTimeUTC,
                Value = Clock.Now.AddMinutes(2).ToString("MM/dd/yyyy HH:mm:ss")
            };

            await ThunderCqrs.SendEvent(
                new FormEndDateNotifyEvent(payload: payload, reminderByConditions: reminderByConditions, model.FormID, model.UserId));
        }

        private async Task PerformSendNotifyStandanloneParticipant(NotifyFormParticipantModel model, FormParticipantNotifyType notifyType)
        {
            string templateName = string.Empty;
            string subject = string.Empty;

            switch (notifyType)
            {
                case FormParticipantNotifyType.Assign:
                    templateName = "FormParticipantAssign";
                    subject = "OPAL2.0 - New form assigned";
                    break;
                case FormParticipantNotifyType.Remind:
                    templateName = "FormParticipantRemind";
                    subject = "OPAL2.0 - Form reminder";
                    break;
                case FormParticipantNotifyType.Remove:
                    templateName = "FormParticipantRemove";
                    subject = "OPAL2.0 - Form removal";
                    break;
                default:
                    break;
            }

            var payload = new NotifyFormParticipantPayload
            {
                FormDetailUrl = _webAppLinkBuilder.GetStandaloneFormPlayerLink(model.FormOriginalObjectId),
                CreatorName = model.FormOwnerName,
                FormName = model.FormTitle,
                UserName = model.ParticipantName,
                ActionUrl = _webAppLinkBuilder.GetStandaloneFormPlayerLink(),
                ObjectType = EventObjectTypeConstant.StandaloneForm,
                ObjectId = model.FormOriginalObjectId
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
