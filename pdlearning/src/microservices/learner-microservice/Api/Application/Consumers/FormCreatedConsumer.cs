using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.form.created")]
    [OpalConsumer("microservice.events.form.rollback")]
    public class FormCreatedConsumer : ScopedOpalMessageConsumer<FormChangeMessage>
    {
        private readonly IRepository<Form> _formRepository;

        public FormCreatedConsumer(IRepository<Form> formRepository)
        {
            _formRepository = formRepository;
        }

        public async Task InternalHandleAsync(FormChangeMessage message)
        {
            var anyExistingForm = await _formRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .AnyAsync();

            if (anyExistingForm)
            {
                return;
            }

            var form = new Form
            {
                Id = message.Id,
                ChangedDate = message.ChangedDate,
                CreatedDate = message.CreatedDate,
                DueDate = message.DueDate,
                EndDate = message.EndDate,
                IsArchived = message.IsArchived,
                IsStandalone = message.IsStandalone,
                OriginalObjectId = message.OriginalObjectId,
                StandaloneMode = message.StandaloneMode,
                StartDate = message.StartDate,
                Status = message.Status,
                SurveyType = message.SurveyType,
                Title = message.Title,
                Type = message.Type
            };

            await _formRepository.InsertAsync(form);
        }
    }
}
