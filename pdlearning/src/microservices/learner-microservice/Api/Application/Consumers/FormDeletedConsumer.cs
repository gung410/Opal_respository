using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.form.deleted")]
    public class FormDeletedConsumer : ScopedOpalMessageConsumer<FormChangeMessage>
    {
        private readonly IReadOnlyRepository<Form> _readFormRepository;
        private readonly IWriteOnlyRepository<Form> _writeFormRepository;
        private readonly IWriteMyOutstandingTaskLogic _myOutstandingTaskCudLogic;

        public FormDeletedConsumer(
            IReadOnlyRepository<Form> readFormRepository,
            IWriteOnlyRepository<Form> writeFormRepository,
            IWriteMyOutstandingTaskLogic myOutstandingTaskCudLogic)
        {
            _readFormRepository = readFormRepository;
            _writeFormRepository = writeFormRepository;
            _myOutstandingTaskCudLogic = myOutstandingTaskCudLogic;
        }

        public async Task InternalHandleAsync(FormChangeMessage message)
        {
            var existingForm = await _readFormRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .FirstOrDefaultAsync();

            if (existingForm == null)
            {
                return;
            }

            // Delete standalone form
            if (existingForm.IsStandaloneForm())
            {
                await _myOutstandingTaskCudLogic.DeleteManyStandaloneFormTask(existingForm.OriginalObjectId);
            }

            await _writeFormRepository.DeleteAsync(existingForm);
        }
    }
}
