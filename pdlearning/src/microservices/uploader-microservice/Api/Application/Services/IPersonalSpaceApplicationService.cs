using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Uploader.Application.Models;
using Microservice.Uploader.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Uploader.Application.Services
{
    public interface IPersonalSpaceApplicationService
    {
        Task<PersonalFileModel> GetPersonalFileById(Guid contentId, Guid userId);

        Task CreatePersonalFile(CreatePersonalFilesRequest request);

        Task DeletePersonalFile(Guid contentId, Guid userId);

        Task<PagedResultDto<PersonalFileModel>> SearchPersonalFiles(SearchPersonalFilesRequestDto dto, Guid userId);

        Task<PersonalSpaceModel> GetPersonalSpaceByUserId(Guid userId);
    }
}
