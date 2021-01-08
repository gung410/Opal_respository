using System;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Uploader.Application.Commands;
using Microservice.Uploader.Application.Models;
using Microservice.Uploader.Application.Queries;
using Microservice.Uploader.Application.RequestDtos;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Application.Services
{
    public class PersonalSpaceApplicationService : ApplicationService, IPersonalSpaceApplicationService
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IAccessControlContext _accessControlContext;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public PersonalSpaceApplicationService(
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _unitOfWorkManager = unitOfWorkManager;
            _accessControlContext = accessControlContext;
        }

        public Task<PersonalFileModel> GetPersonalFileById(Guid contentId, Guid userId)
        {
            return _thunderCqrs.SendQuery(new GetPersonalFileByIdQuery
            {
                Id = contentId,
                UserId = userId
            });
        }

        public Task CreatePersonalFile(CreatePersonalFilesRequest request)
        {
            var saveCommand = new SavePersonalFileCommand
            {
                CreationRequest = request
            };

            return this._thunderCqrs.SendCommand(saveCommand);
        }

        public async Task DeletePersonalFile(Guid contentId, Guid userId)
        {
            var deleteFromCommand = new DeletePersonalFileCommand
            {
                Id = contentId,
                UserId = userId
            };

            await _thunderCqrs.SendCommand(deleteFromCommand);
        }

        public Task<PagedResultDto<PersonalFileModel>> SearchPersonalFiles(SearchPersonalFilesRequestDto dto, Guid userId)
        {
            return _thunderCqrs.SendQuery(new SearchPersonalFilesQuery
            {
                PagedInfo = dto.PagedInfo,
                UserId = userId,
                SearchText = dto.SearchText,
                FilterByExtensions = dto.FilterByExtensions,
                FilterByType = dto.FilterByType,
                SortBy = dto.SortBy,
                SortDirection = dto.SortDirection
            });
        }

        public Task<PersonalSpaceModel> GetPersonalSpaceByUserId(Guid userId)
        {
            return _thunderCqrs.SendQuery(new GetPersonalSpaceByUserIdQuery
            {
                UserId = userId
            });
        }
    }
}
