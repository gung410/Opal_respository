using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.BrokenLink.Application.Events;
using Microservice.BrokenLink.Application.Models;
using Microsoft.Extensions.Configuration;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.BrokenLink.Application.Services
{
    public class BrokenLinkNotifier : ApplicationService, IBrokenLinkNotifier
    {
        private readonly IThunderCqrs _thunderCqrs;

        public BrokenLinkNotifier(
            IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
        }

        public async Task NotifyBrokenLinkFound(BrokenLinkReportModel brokenLinkReportModel)
        {
            // TODO: Move the email template name to configuration.
            await PerformSendEmail(brokenLinkReportModel, "ContentBrokenLinkSystemAlert");
        }

        public async Task NotifyLearnerReport(BrokenLinkReportModel brokenLinkReportModel)
        {
            await PerformSendEmail(brokenLinkReportModel, "ContentBrokenLinkLearnerReport");
        }

        private async Task PerformSendEmail(BrokenLinkReportModel brokenLinkReportModel, string emailTemplateName)
        {
            var payload = new NotifyBrokenLinkPayload
            {
                AssetOwnerName = brokenLinkReportModel.ObjectOwnerName,
                AssetName = brokenLinkReportModel.ObjectTitle,
                AssetDetailUrl = brokenLinkReportModel.ObjectDetailUrl,
                ActionUrl = brokenLinkReportModel.ObjectDetailUrl,
                ReporterName = brokenLinkReportModel.ReporterName
            };
            var reminderByConditions = new ReminderByDto
            {
                Type = ReminderByType.AbsoluteDateTimeUTC,

                // Add 2 minutes to ensure the time is valid after the message was sent to Communication.
                Value = Clock.Now.AddMinutes(2).ToString("MM/dd/yyyy HH:mm:ss")
            };

            await _thunderCqrs.SendEvent(
                new NotifyBrokenLinkEvent(
                    payload: payload,
                    ownerId: brokenLinkReportModel.ObjectOwnerId,
                    objectId: brokenLinkReportModel.ObjectId,
                    reminderByConditions: reminderByConditions,
                    template: emailTemplateName));
        }
    }
}
