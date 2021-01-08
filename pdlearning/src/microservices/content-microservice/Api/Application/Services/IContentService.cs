using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Content.Application.Services
{
    public interface IContentService
    {
        Task<DigitalContentModel> CreateDigitalContent(CreateDigitalContentRequest request, Guid userId);

        Task<DigitalContentModel> UpdateDigitalContent(UpdateDigitalContentRequest request, Guid userId);

        Task<PagedResultDto<SearchDigitalContentModel>> SearchDigitalContent(SearchDigitalContentRequest dto, Guid userId);

        Task<PagedResultDto<SearchDigitalContentModel>> GetPendingApprovalDigitalContents(GetPendingApprovalDigitalContentsRequest dto);

        Task<DigitalContentModel> GetDigitalContentById(Guid contentId, Guid userId);

        Task<List<DigitalContentModel>> GetListDigitalContentsByListIds(List<Guid> listIds);

        Task DeleteDigitalContent(Guid contentId, Guid userId);

        Task RenameDigitalContent(RenameDigitalContentRequest request, Guid userId);

        Task<DigitalContentModel> CloneDigitalContent(Guid contentId, Guid userId);

        Task ChangeApprovalStatus(ChangeApprovalStatusRequest request, Guid userId);

        Task<DigitalContentExpiryInfoModel[]> GetExpiryInfoOfDigitalContents(Guid[] contentIds);

        Task TransferCourseOwnership(TransferOwnershipRequest request);

        Task MigrateContentNotification(List<Guid> listIds);

        Task<DigitalContentModel> GetDigitalContentByVersionTrackingId(GetContentByVersionTrackingIdRequestDto request, Guid userId);

        Task<PagedResultDto<Guid>> MigrateSearchContentData(MigrateSearchContentDataRequest request);
    }
}
