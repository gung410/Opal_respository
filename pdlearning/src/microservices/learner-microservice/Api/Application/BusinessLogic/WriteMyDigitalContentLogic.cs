using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.Events;
using Microservice.Learner.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.BusinessLogic
{
    public class WriteMyDigitalContentLogic : BaseBusinessLogic<MyDigitalContent>, IWriteMyDigitalContentLogic
    {
        public WriteMyDigitalContentLogic(
            IThunderCqrs thunderCqrs,
            IWriteOnlyRepository<MyDigitalContent> writeMyDigitalContentRepository)
            : base(thunderCqrs, writeMyDigitalContentRepository)
        {
        }

        public async Task Insert(MyDigitalContent myDigitalContent)
        {
            await SendLearningMessage(myDigitalContent);

            await WriteRepository.InsertAsync(myDigitalContent);
        }

        public async Task Update(MyDigitalContent myDigitalContent)
        {
            await SendLearningMessage(myDigitalContent);

            await WriteRepository.UpdateAsync(myDigitalContent);
        }

        private async Task SendLearningMessage(MyDigitalContent myDigitalContent)
        {
            await ThunderCqrs.SendEvent(
                new MyDigitalContentChangeEvent(myDigitalContent, myDigitalContent.Status));

            // Support for report module
            await ThunderCqrs.SendEvent(
                new MyDigitalContentRecordEvent(myDigitalContent));
        }
    }
}
