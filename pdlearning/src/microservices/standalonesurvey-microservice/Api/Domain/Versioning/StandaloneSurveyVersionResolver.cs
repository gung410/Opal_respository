using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Commands;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Versioning.Application.Commands;
using Microservice.StandaloneSurvey.Versioning.Core;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Domain.Versioning
{
    public class StandaloneSurveyVersionResolver : SchemaVersionResolver<StandaloneSurveyModel, Entities.StandaloneSurvey>
    {
        private readonly IThunderCqrs _thunderCqrs;

        public StandaloneSurveyVersionResolver(IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
        }

        public override async Task DoLogicCheckoutVersion(
            RevertVersionCommand revertVersionCommand,
            VersionTracking versionTrackingData,
            StandaloneSurveyModel dataVersion)
        {
            // Mark current active record as archived version
            MarkFormAsArchivedCommand markFormAsArchivedCommand = new MarkFormAsArchivedCommand()
            {
                Id = revertVersionCommand.Request.CurrentActiveId,
                UserId = revertVersionCommand.UserId
            };

            await _thunderCqrs.SendCommand(markFormAsArchivedCommand);

            // Clone backup record to active version
            var rollbackCommand = new RollbackFormCommand
            {
                RevertFromRecordId = revertVersionCommand.Request.RevertFromRecordId,
                RevertToRecordId = revertVersionCommand.NewActiveId,
                CurrentActiveId = revertVersionCommand.Request.CurrentActiveId,
                UserId = revertVersionCommand.UserId,
            };
            await _thunderCqrs.SendCommand(rollbackCommand);
        }
    }
}
