using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Webinar.Application.Commands.CommandHandlers
{
    public class UpdateRecordCommandHandler : BaseCommandHandler<UpdateRecordCommand>
    {
        private readonly IRepository<Record> _recordRepository;

        public UpdateRecordCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IRepository<Record> recordRepository) : base(unitOfWorkManager, userContext)
        {
            _recordRepository = recordRepository;
        }

        protected override async Task HandleAsync(UpdateRecordCommand command, CancellationToken cancellationToken)
        {
            var record = await _recordRepository
                .FirstOrDefaultAsync(x => x.MeetingId == command.MeetingId && x.InternalMeetingId == command.InternalMeetingId);
            if (record == null)
            {
                throw new EntityNotFoundException();
            }

            record.Status = command.Status;
            await _recordRepository.UpdateAsync(record);
        }
    }
}
