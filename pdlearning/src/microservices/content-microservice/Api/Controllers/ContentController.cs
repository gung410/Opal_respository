using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Content.Application.Commands;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.RequestDtos;
using Microservice.Content.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Content.Controllers
{
    [Route("api/contents")]
    public partial class ContentController : ApplicationApiController
    {
        private readonly IContentService _appService;
        private readonly IThunderCqrs _thunderCqrs;

        public ContentController(IThunderCqrs thunderCqrs, IContentService appService, IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
            _appService = appService;
        }

        [HttpGet("{contentId:guid}")]
        public async Task<DigitalContentModel> GetDigitalContentById(Guid contentId)
        {
            return await _appService.GetDigitalContentById(contentId, CurrentUserId);
        }

        [HttpPost("getByIds")]
        public async Task<IEnumerable<DigitalContentModel>> GetListContentsByListIds([FromBody] List<Guid> ids)
        {
            return await _appService.GetListDigitalContentsByListIds(ids);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<SearchDigitalContentModel>> SearchDigitalContent([FromBody] SearchDigitalContentRequest request)
        {
            return await _appService.SearchDigitalContent(request, CurrentUserId);
        }

        [HttpPost("getPendingApprovalDigitalContents")]
        public async Task<PagedResultDto<SearchDigitalContentModel>> GetPendingApprovalDigitalContents([FromBody] GetPendingApprovalDigitalContentsRequest request)
        {
            return await _appService.GetPendingApprovalDigitalContents(request);
        }

        [PermissionRequiredAttribute(CourseContentPermissionKeys.DigitalContentCreate)]
        [HttpPost("create")]
        public async Task<DigitalContentModel> CreateDigitalContent([FromBody] CreateDigitalContentRequest request)
        {
            return await _appService.CreateDigitalContent(request, CurrentUserId);
        }

        [HttpPut("changeApprovalStatus")]
        public async Task ChangeApprovalStatus([FromBody] ChangeApprovalStatusRequest request)
        {
            await _appService.ChangeApprovalStatus(request, CurrentUserId);
        }

        [PermissionRequiredAttribute(CourseContentPermissionKeys.DigitalContentDuplicate)]
        [HttpPut("{contentId:guid}/clone")]
        public async Task<DigitalContentModel> CloneDigitalContent(Guid contentId)
        {
            return await _appService.CloneDigitalContent(contentId, CurrentUserId);
        }

        [HttpPut("update")]
        public async Task<DigitalContentModel> UpdateDigitalContent([FromBody]UpdateDigitalContentRequest request)
        {
            return await _appService.UpdateDigitalContent(request, CurrentUserId);
        }

        [PermissionRequiredAttribute(CourseContentPermissionKeys.DigitalContentRename)]
        [HttpPut("rename")]
        public async Task RenameDigitalContent([FromBody] RenameDigitalContentRequest request)
        {
            await _appService.RenameDigitalContent(request, CurrentUserId);
        }

        [PermissionRequiredAttribute(CourseContentPermissionKeys.DigitalContentDelete)]
        [HttpDelete("{contentId:guid}")]
        public async Task DeleteDigitalContent(Guid contentId)
        {
            await _appService.DeleteDigitalContent(contentId, CurrentUserId);
        }

        [HttpPost("GetExpiryInfoOfDigitalContents")]
        public async Task<DigitalContentExpiryInfoModel[]> GetExpiryInfoOfDigitalContents([FromBody] Guid[] listDigitalContentId)
        {
            return await _appService.GetExpiryInfoOfDigitalContents(listDigitalContentId);
        }

        [PermissionRequiredAttribute(CourseContentPermissionKeys.DigitalContentTransferOwnerShip)]
        [HttpPut("transfer")]
        public async Task TransferOwnership([FromBody] TransferOwnershipRequest request)
        {
            await _appService.TransferCourseOwnership(request);
        }

        [PermissionRequiredAttribute(CourseContentPermissionKeys.DigitalContentArchive)]
        [HttpPut("archive")]
        public async Task Archive([FromBody] ArchiveContentRequest request)
        {
            var command = new ArchiveDigitalContentCommand
            {
                ContentId = request.ObjectId,
                ArchiveBy = request.ArchiveByUserId
            };

            await _thunderCqrs.SendCommand(command);
        }

        [HttpGet("byVersionTrackingId/{versionTrackingId:guid}")]
        public async Task<DigitalContentModel> GetContentByVersionTrackingId(Guid versionTrackingId)
        {
            return await _appService.GetDigitalContentByVersionTrackingId(
                new GetContentByVersionTrackingIdRequestDto { VersionTrackingId = versionTrackingId },
                CurrentUserId);
        }

        [HttpPost("migrateSearchContentData")]
        public async Task<PagedResultDto<Guid>> MigrateSearchContentData([FromBody] MigrateSearchContentDataRequest request)
        {
            if (!UserContext.IsSysAdministrator())
            {
                throw new BusinessLogicException("You do not have permission to access this endpoint");
            }

            return await _appService.MigrateSearchContentData(request);
        }
    }
}
