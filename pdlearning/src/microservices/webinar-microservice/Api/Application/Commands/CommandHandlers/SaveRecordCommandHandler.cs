using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Webinar.Application.Commands.CommandHandlers
{
    public class SaveRecordCommandHandler : BaseCommandHandler<SaveRecordCommand>
    {
        private readonly IRepository<Record> _recordRepository;

        public SaveRecordCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IRepository<Record> recordRepository) : base(unitOfWorkManager, userContext)
        {
            _recordRepository = recordRepository;
        }

        protected override async Task HandleAsync(SaveRecordCommand command, CancellationToken cancellationToken)
        {
            var record = new Record()
            {
                Id = command.Id,
                MeetingId = command.MeetingId,
                Status = command.Status,
                RecordId = command.RecordId,
                InternalMeetingId = command.InternalMeetingId
            };
            await _recordRepository.InsertAsync(record);
        }
    }
}
