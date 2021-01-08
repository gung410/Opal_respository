using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Uploader.Domain.Entities;
using Microservice.Uploader.Infrastructure;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Uploader.Application.Commands.CommandHandlers
{
    public class SavePersonalFileCommandHandler : BaseCommandHandler<SavePersonalFileCommand>
    {
        private readonly IRepository<PersonalFile> _personalFileRepository;
        private readonly IRepository<PersonalSpace> _personalSpaceRepository;

        public SavePersonalFileCommandHandler(
            IRepository<PersonalFile> personalFileRepository,
            IRepository<PersonalSpace> personalSpaceRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, accessControlContext)
        {
            _personalFileRepository = personalFileRepository;
            _personalSpaceRepository = personalSpaceRepository;
        }

        protected override async Task HandleAsync(SavePersonalFileCommand command, CancellationToken cancellationToken)
        {
            var fileRequest = command.CreationRequest.PersonalFiles;
            if (fileRequest.Count == 0)
            {
               throw new PersonalSpaceAccessDeniedException();
            }

            var existedPersonalSpace = _personalSpaceRepository.FirstOrDefault(PersonalSpaceExpressions.HasPermissionToSeePersonalSpace(CurrentUserId));
            if (existedPersonalSpace == null)
            {
                throw new PersonalSpaceAccessDeniedException();
            }

            var toInsertFile = new List<PersonalFile>();
            for (var i = 0; i < fileRequest.Count; i++)
            {
                var personalFile = new PersonalFile
                {
                    FileExtension = fileRequest[i].FileExtension,
                    FileLocation = fileRequest[i].FileLocation,
                    FileName = fileRequest[i].FileName,
                    FileSize = fileRequest[i].FileSize,
                    FileType = fileRequest[i].FileType,
                    PersonalSpaceId = existedPersonalSpace.Id,
                    UserId = CurrentUserId
                };

                toInsertFile.Add(personalFile);
            }

            await _personalFileRepository.InsertManyAsync(toInsertFile);

            existedPersonalSpace.TotalUsed += toInsertFile.Sum(x => x.FileSize);
            await _personalSpaceRepository.UpdateAsync(existedPersonalSpace);
        }
    }
}
