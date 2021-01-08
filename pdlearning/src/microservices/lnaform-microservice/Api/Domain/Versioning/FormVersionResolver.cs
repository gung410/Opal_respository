using System.Threading.Tasks;
using Microservice.LnaForm.Application.Commands;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Versioning.Application.Commands;
using Microservice.LnaForm.Versioning.Core;
using Microservice.LnaForm.Versioning.Entities;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Domain.Versioning
{
    public class FormVersionResolver : SchemaVersionResolver<FormModel, FormEntity>
    {
        private readonly IThunderCqrs _thunderCqrs;

        public FormVersionResolver(IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
        }

        public override async Task DoLogicCheckoutVersion(
            RevertVersionCommand revertVersionCommand,
            VersionTracking versionTrackingData,
            FormModel dataVersion)
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
