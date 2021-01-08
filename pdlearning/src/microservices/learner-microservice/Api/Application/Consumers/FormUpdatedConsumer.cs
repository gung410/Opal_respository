using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.form.updated")]
    [OpalConsumer("microservice.events.form.archived")]
    public class FormUpdatedConsumer : ScopedOpalMessageConsumer<FormChangeMessage>
    {
        private readonly IRepository<Form> _formRepository;
        private readonly IWriteMyOutstandingTaskLogic _myOutstandingTaskCudLogic;

        public FormUpdatedConsumer(
            IRepository<Form> formRepository,
            IWriteMyOutstandingTaskLogic myOutstandingTaskCudLogic)
        {
            _formRepository = formRepository;
            _myOutstandingTaskCudLogic = myOutstandingTaskCudLogic;
        }

        public async Task InternalHandleAsync(FormChangeMessage message)
        {
            var existingForm = await _formRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .FirstOrDefaultAsync();

            if (existingForm == null)
            {
                return;
            }

            // Remove tasks if the form is not standalone
            // or the current version of standalone form is archived
            // or the status of standalone form is archived.
            if (CanDeleteOutstandingTask(message, existingForm))
            {
                await _myOutstandingTaskCudLogic.DeleteManyStandaloneFormTask(message.OriginalObjectId);
            }

            // Add tasks if the standalone form is enabled or published
            if (CanInsertOutstandingTask(message, existingForm))
            {
                await _myOutstandingTaskCudLogic.InsertManyStandaloneFormTask(message.OriginalObjectId);
            }

            existingForm.Update(
                message.Title,
                message.Type,
                message.Status,
                message.SurveyType,
                message.DueDate,
                message.OriginalObjectId,
                message.IsArchived,
                message.StartDate,
                message.EndDate,
                message.StandaloneMode,
                message.IsStandalone);

            await _formRepository.UpdateAsync(existingForm);
        }

        private bool CanInsertOutstandingTask(FormChangeMessage message, Form existingForm)
        {
            return (!existingForm.IsStandaloneForm() && message.IsStandaloneForm())
                   || (!existingForm.IsStandaloneFormPublished() && message.IsStandaloneFormPublished());
        }

        private bool CanDeleteOutstandingTask(FormChangeMessage message, Form existingForm)
        {
            return message.IsArchivedStandaloneVersionForm() ||
                   (existingForm.IsStandaloneForm() && !message.IsStandaloneForm())
                   || (existingForm.IsStandaloneForm() && message.IsArchivedForm());
        }
    }
}
